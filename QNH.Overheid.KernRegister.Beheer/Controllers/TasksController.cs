using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Win32.TaskScheduler;
using NLog;
using QNH.Overheid.KernRegister.Beheer.Utilities;
using QNH.Overheid.KernRegister.Business.Business;
using QNH.Overheid.KernRegister.Business.Service.Users;
using QNH.Overheid.KernRegister.Business.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    public class MutatiesModel
    {
        public IEnumerable<string> MutatiesLogFiles
        {
            get
            {
                var directory = HostingEnvironment.MapPath("~/Logs/Mutaties");
                if (Directory.Exists(directory))
                    return Directory.GetFiles(directory, "*.log");
                else
                    return Enumerable.Empty<string>();
            }
        }
    }

    [CVnHRAuthorize(ApplicationActions.CVnHR_Tasks)]
    public class TasksController : Controller
    {
        // GET: Tasks
        public ActionResult Index()
        {
            return View(this);
        }

        public ActionResult Mutaties()
        {
            return View(new MutatiesModel());
        }

        public ActionResult DownloadMutatieCsvOutsideArea()
        {
            throw new NotImplementedException("TODO, impelment me!");
        }

        [HttpPost]
        public void DeleteTask(string name) {
            string executablePath;
            executablePath = ConfigurationManager.AppSettings["BatchProcessExecutablePath"];
            //determine whether path is absolute or relative
            if (!executablePath.Contains(":"))
            {
                executablePath = Path.Combine(
                        Directory.GetParent(
                            Directory.GetParent(
                                AppDomain.CurrentDomain.BaseDirectory
                                ).ToString()
                             ).ToString()
                             , executablePath);
            }
            string exePath = Path.Combine(new FileInfo(executablePath).DirectoryName,"JsonConfig");
            TaskService ts = new TaskService();
            ts.RootFolder.DeleteTask(name);
            string filePath = Path.Combine(exePath, name.Substring(name.IndexOf(' ') + 1) + "-brmo-config.json");
            if (System.IO.File.Exists(filePath)) {
                System.IO.File.Delete(filePath);
            }
        }

        public IEnumerable<Task> TaskProcesses
        {
            get
            {
                TaskService ts = new TaskService();
                IEnumerable<Task> taskCollection = ts.RootFolder.GetTasks(new System.Text.RegularExpressions.Regex(@"CVnHR")).ToArray();
                List<Task> taskList = new List<Task>();
                foreach (Task t in taskCollection) {
                    if (t.Definition.Actions.Cast<ExecAction>().Single().Arguments.Split(' ')[0].Equals("BRMO")) {
                        taskList.Add(t);
                    }
                }
                return taskList;

            }
        }

        public FileResult DownloadLogFile(string fileName)
        {
            var model = new MutatiesModel();
            var file = model.MutatiesLogFiles.SingleOrDefault(f => f == fileName || f.EndsWith(fileName));
            if (file != null)
            {
                return File(file, "text/plain");
            }
            else
                return null;
        }
    }

    [CVnHRAuthorize(ApplicationActions.CVnHR_Tasks)]
    [HubName("csvMutatieHub")]
    public class CsvMutatieHub : Hub
    {
        private static readonly Logger _logger = LogManager.GetLogger("mutatiesLogger");
        private static readonly Logger _summaryLogger = LogManager.GetLogger("mutatiesSummaryLogger");

        public void ProcessCsv(string fileToProcess, bool brmoChecked, bool cvnhrChecked)
        {
            try
            {
                var physicalPath = HostingEnvironment.MapPath("~/Files/mutaties/" + fileToProcess);
                if (physicalPath == null)
                {
                    return;
                }

                var file = new FileInfo(physicalPath);
                if (!file.Exists)
                {
                    return;
                }

                if (file.Extension.ToLower() != ".csv")
                {
                    return;
                }

                var records = CsvUtils.ReadInschrijvingRecords(file.FullName, _logger);
                var maxDegreeOfParallelism = Convert.ToInt32(ConfigurationManager.AppSettings["MaxDegreeOfParallelism"] ?? "1");

                var startStopLogger = LogManager.GetCurrentClassLogger();

                startStopLogger.Debug($"Starting to process file {file.FullName} for CVnHR: {cvnhrChecked} and for BRMO: {brmoChecked}");
                _summaryLogger.Info($"Starting to process file {file.FullName} for CVnHR: {cvnhrChecked} and for BRMO: {brmoChecked}");

                var processing = new InschrijvingProcessing(IocConfig.Container, maxDegreeOfParallelism, _logger);
                processing.RecordProcessed += RecordProcessedHandler;

                if (cvnhrChecked)
                {
                    _logger.Info($"Starting CVnHR upload for file {file.FullName}");
                    processing.ProcessRecords(records, Context.User.GetUserName(), ProcessTaskTypes.CVnHR);
                }
                if (brmoChecked)
                {
                    _logger.Info($"Starting BRMO upload for file {file.FullName}");
                    processing.ProcessRecords(records, Context.User.GetUserName(), ProcessTaskTypes.Brmo);
                }
                startStopLogger.Debug($"Finished!");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error on processing file! {fileToProcess}");
            }
        }

        public void RecordProcessedHandler(object sender, RecordProcessedEventArgs e)
        {
            // Clients.All.reportProgress(e.Progress, e.InschrijvingNaam);
            Debug.Print($"Progress={e.Progress} Inschrijvingsnaam={e.InschrijvingNaam}");
            Clients.Caller.reportProgress(e.SuccesCount, e.ErrorCount, e.Progress, e.SuccesProgress, e.InschrijvingNaam, e.TotalNew, e.TotalUpdated, e.TotalAlreadyExisted);

            if (e.InschrijvingNaam == "Klaar") // 100%
            {
                _summaryLogger.Info("-------------------------------------------");
                _summaryLogger.Info($@"Summary {e.Type}: 
Success: {e.SuccesCount},
Error: {e.ErrorCount},
TotalNew: {e.TotalNew},
TotalUpdated: {e.TotalUpdated},
TotalAlreadyExisted: {e.TotalAlreadyExisted}");

                if (e.Errors.Count > 0)
                {
                    _summaryLogger.Error($"Errors: {Environment.NewLine}{string.Join(Environment.NewLine, e.Errors)}");
                }

                _summaryLogger.Info("-------------------------------------------");
            }
        }
    }
}