using Backload.Controllers;
using CsvHelper;
using QNH.Overheid.KernRegister.Beheer.Migrations;
using QNH.Overheid.KernRegister.Business.Business;
using QNH.Overheid.KernRegister.Business.Model;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Win32.TaskScheduler;
using NLog;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;
using QNH.Overheid.KernRegister.Beheer.Utilities;
using QNH.Overheid.KernRegister.Business.Crm;
using QNH.Overheid.KernRegister.Organization.Resources;
using QNH.Overheid.KernRegister.Business.Service.Users;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    [CVnHRAuthorize(ApplicationActions.CVnHR_SyncCrm)]
    public class ExportController : TaskSchedulerPartialController
    {        
        //private IUnitOfWork _unitOfWork;

        public ExportController()//(IUnitOfWork unitOfWork)
            : base(Default.ApplicationName + " Batchupdate DocBase Vestigingen Task", "B")
        {
            //_unitOfWork = unitOfWork;
        }

        //
        // GET: /Export/
        public ActionResult Index()
        {
            if (ExportTaskManager == null)
                throw new ArgumentException("ExportTaskManager");

            return View(ExportTaskManager);
        }
        
    }

    [CVnHRAuthorize(ApplicationActions.CVnHR_SyncCrm)]
    [HubName("csvExportHub")]
    public class CsvExportHub : Hub
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Exports the kvknummers found in the cv
        /// </summary>
        /// <param name="fileToExport"></param>
        public void ExportCsv(string fileToExport)
        {
            logger.Debug("Starting to export file: " + fileToExport);

            var physicalPath = HostingEnvironment.MapPath("~/Files/export/" + fileToExport);
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

            var repo = IocConfig.Container.GetInstance<IKvkInschrijvingRepository>();
            var exportService = IocConfig.Container.GetInstance<IExportService>();

            // Start the processing
            var processing = new ExportProcessing(repo, exportService);
            processing.RecordProcessed += (sender, e) => {
                Debug.Print(String.Format("Progress={0} Inschrijvingsnaam={1}", e.Progress, e.InschrijvingNaam));
                Clients.All.reportProgress(e.SuccesCount, e.ErrorCount, e.Progress, e.SuccesProgress, e.InschrijvingNaam, e.TotalNew, e.TotalUpdated, e.TotalAlreadyExisted);
            };
            processing.RecordsToInsertFound += (sender, e) =>
            {
                Clients.All.reportItemsToInsert(e.RecordsToInsert);
            };
            processing.ProcessRecords(records, Default.CrmApplication);
        }

        public void InsertItems(IEnumerable<string> itemsToInsert)
        {
            logger.Debug("Starting to insert items: " + string.Join(", ", itemsToInsert));

            var repo = IocConfig.Container.GetInstance<IKvkInschrijvingRepository>();
            var exportService = IocConfig.Container.GetInstance<IExportService>();

            // Start the processing
            var processing = new ExportProcessing(repo, exportService);
            processing.RecordProcessed += (sender, e) =>
            {
                Debug.Print(String.Format("Progress={0} Inschrijvingsnaam={1}", e.Progress, e.InschrijvingNaam));
                Clients.All.reportProgress(e.SuccesCount, e.ErrorCount, e.Progress, e.SuccesProgress, e.InschrijvingNaam, e.TotalNew, e.TotalUpdated, e.TotalAlreadyExisted);
            };
            processing.InsertRecords(itemsToInsert.Select(i=> new InschrijvingRecord { kvknummer = i }));
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
