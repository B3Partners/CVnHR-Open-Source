using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using CsvHelper;
using QNH.Overheid.KernRegister.Business.Business;
using QNH.Overheid.KernRegister.Business.Model;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using NLog;
using System.Configuration;
using QNH.Overheid.KernRegister.Organization.Resources;
using QNH.Overheid.KernRegister.Business.Service.Users;
using QNH.Overheid.KernRegister.Beheer.Utilities;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    [CVnHRAuthorize(ApplicationActions.CVnHR_SyncKvKData)]
    public class InschrijvingController : TaskSchedulerPartialController
    {

        public InschrijvingController()//(IUnitOfWork unitOfWork)
            : base(Default.ApplicationName + " Batchupdate From KvK Task", "K")
        {
        }
        //
        // GET: /Inschrijving/

        public ActionResult Index()
        {
            return View(ExportTaskManager);
        }
    }

    [CVnHRAuthorize(ApplicationActions.CVnHR_ManageKvKData)]
    [HubName("csvImportHub")]
    public class CsvImportHub : Hub
    {
        public void ProcessCsv(string fileToProcess)
        {
#warning //TODO: Ensure the hub on connected/ on disconnected keeps working or stops.

            var physicalPath = HostingEnvironment.MapPath("~/Files/inschrijving/" + fileToProcess);
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
            var records = ReadInschrijvingRecords(file.FullName);
            var maxDegreeOfParallelism = Convert.ToInt32(ConfigurationManager.AppSettings["MaxDegreeOfParallelism"] ?? "1");
            var processing = new InschrijvingProcessing(IocConfig.Container, maxDegreeOfParallelism);
            processing.RecordProcessed += RecordProcessedHandler;
            processing.ProcessRecords(records, Context.User.GetUserName());
        }

        public void RecordProcessedHandler(object sender, RecordProcessedEventArgs e)
        {
            // Clients.All.reportProgress(e.Progress, e.InschrijvingNaam);
            Debug.Print($"Progress={e.Progress} Inschrijvingsnaam={e.InschrijvingNaam}");
            Clients.All.reportProgress(e.SuccesCount, e.ErrorCount,  e.Progress, e.SuccesProgress,  e.InschrijvingNaam, e.TotalNew, e.TotalUpdated, e.TotalAlreadyExisted);
        }

        private IEnumerable<InschrijvingRecord> ReadInschrijvingRecords(string fileName)
        {
            // First read complete CSV to see how many KVKnummers we need to process
            IEnumerable<InschrijvingRecord> inschrijvingCsvRecords;
            using (TextReader reader = File.OpenText(fileName))
            {
                var csv = new CsvReader(reader);
                inschrijvingCsvRecords = csv.GetRecords<InschrijvingRecord>().ToArray();
            }

            return inschrijvingCsvRecords;
        }
    }
}
