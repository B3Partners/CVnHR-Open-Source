using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json;
using NLog;
using QNH.Overheid.KernRegister.Beheer.Utilities;
using QNH.Overheid.KernRegister.Business.Business;
using QNH.Overheid.KernRegister.Business.Service.Users;
using QNH.Overheid.KernRegister.Business.Service.ZipCodes;
using QNH.Overheid.KernRegister.Business.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using JsonFile = System.IO.File;

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

        public IEnumerable<string> ZipCodes { get; set; }
    }

    [CVnHRAuthorize(ApplicationActions.CVnHR_Tasks)]
    public class TasksController : Controller
    {

        private readonly IAreaService _areaService;

        public TasksController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        // GET: Tasks
        public ActionResult Index()
        {
            return View(this);
        }

        public ActionResult Mutaties()
        {
            return View(new MutatiesModel() { ZipCodes = GetZipCodes() });
        }

        [HttpGet]
        public ActionResult DownloadMutatieCsvOutsideArea()
        {
            var kvkNummers = _areaService.GetInschrijvingenWithAllVestigingenOutsideArea(GetZipCodes())
                .Select(KvkNummer => new { KvkNummer });

            return File(CsvUtils.WriteToCsv(kvkNummers), "text/csv", GetMutatieCsvOutsideAreaFileName());
        }

        [HttpGet]
        public ActionResult DownloadMutatieCsvOutsideAreaAndProcess()
        {
            var kvkNummers = _areaService.GetInschrijvingenWithAllVestigingenOutsideArea(GetZipCodes())
                .Select(KvkNummer => new { KvkNummer });

            var fileName = GetMutatieCsvOutsideAreaFileName();
            var physicalPath = HostingEnvironment.MapPath("~/Files/mutaties/" + fileName);

            JsonFile.WriteAllBytes(physicalPath, CsvUtils.WriteToCsv(kvkNummers));

            return Content(fileName);
        }

        private string GetMutatieCsvOutsideAreaFileName()
            => $"{DateTime.Now.ToString("yyyy-MM-dd-HHmmss")}-Inschrijvingen-buiten-geconfigureerd-gebied.csv";

        [HttpPost]
        public ActionResult SaveZipCodeConfiguration(string zipCodes)
        {
            var postCodes = zipCodes
               .Split(new[] { ",", ";", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
               .Select(p => p.Trim())
               .Distinct();

            JsonFile.WriteAllText(GetZipCodePath(), JsonConvert.SerializeObject(postCodes));

            return new EmptyResult();
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
            if (JsonFile.Exists(filePath)) {
                JsonFile.Delete(filePath);
            }
        }

        public IEnumerable<Task> TaskProcesses
        {
            get
            {
                TaskService ts = new TaskService();
                IEnumerable<Task> taskCollection = ts.RootFolder.GetTasks(new Regex(@"CVnHR")).ToArray();
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

        public ActionResult GetZipcodesDrentheGroningenEnAangrezendeGemeenten() {
            return Content(string.Join(", ", ZipcodesDrentheGroningenEnAangrezendeGemeenten.AllCombined));
        }

        private string GetZipCodeConfigDirectory() =>
            HostingEnvironment.MapPath("~/JsonConfig");

        private string GetZipCodePath()
        {
            return Path.Combine(GetZipCodeConfigDirectory(), "mutaties-zipcodes.json");
        }

        private IEnumerable<string> GetZipCodes()
        {
            var zipCodeDir = GetZipCodeConfigDirectory();
            if (!Directory.Exists(zipCodeDir))
            {
                Directory.CreateDirectory(zipCodeDir);
            }

            var path = GetZipCodePath();
            if (!JsonFile.Exists(path))
            {
                JsonFile.WriteAllText(path, JsonConvert.SerializeObject(new[] { "0000", "0001" }));
            }

            return JsonConvert.DeserializeObject<IEnumerable<string>>(JsonFile.ReadAllText(path));
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