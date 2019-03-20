using QNH.Overheid.KernRegister.Business.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QNH.Overheid.KernRegister.Business.Crm;
using QNH.Overheid.KernRegister.Business.Utility;

namespace QNH.Overheid.KernRegister.Business.Business
{
    // TODO: completely rewrite and/or refactor / may be deleted even...
    public class ExportProcessing
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IKvkInschrijvingRepository _repo;
        private readonly IExportService _exportService;

        public ExportProcessing(IKvkInschrijvingRepository repo, IExportService exportService)
        {
            _exportService = exportService;
            _repo = repo;
        }

        public void ProcessRecords(IEnumerable<InschrijvingRecord> inschrijvingRecords, string crmName)
        { 
            double incrWith = 100.0 / inschrijvingRecords.Count();

            double progress = 0;
            double succesprogress = 0;

            int loopcount = 0;
            int succesCount = 0;
            int errorCount = 0;
            int totalNew = 0;
            int totalUpdated = 0;
            int totalAlreadyExisted = 0;

            var itemsToInsert = new List<string>();
            foreach (var inschrijvingCsvRecord in inschrijvingRecords)
            {
                loopcount += 1;

                var inschrijving = _repo.GetLatestInschrijving(inschrijvingCsvRecord.kvknummer);
                IExportResult exportResult = null;
                string msg;
                if (inschrijving == null)
                {
                    msg = $"No inschrijving exists in kernregister for kvkNummer: {inschrijvingCsvRecord.kvknummer}";
                    logger.Warn(msg);
                }
                else
                {
                    try
                    {
                        exportResult = _exportService.UpdateExternalRecord(inschrijving);
                    }
                    catch (Exception ex)
                    {
                        msg =
                            $"Er ging wat mis bij het updaten van de export record voor KVK register database KVK nummer {inschrijvingCsvRecord.kvknummer}";
                        logger.Warn(msg);
                        logger.ErrorException(
                            $"Error while updating KVKNummer = {inschrijving.KvkNummer} Naam = {inschrijving.InschrijvingNaam} Check the exception", ex);
                    }

                    if (exportResult != null)
                    {
                        if (exportResult.Succes)
                        {
                            succesCount++;
                            succesprogress += incrWith;
                            totalUpdated++;
                            msg =
                                $"Succesfully updated export record voor kvknumer = {inschrijving.KvkNummer}, naam = {inschrijving.InschrijvingNaam}, aantal vestigingen = {inschrijving.Vestigingen.Count}";
                            logger.Info(msg);
                        }
                        else if (!exportResult.Succes && exportResult.NoItemsFoundInsertInstead) // The record does not yet exist in the export database
                        {
                            itemsToInsert.Add(inschrijving.KvkNummer);
                            totalNew++;
                            msg =
                                $"Could not update record voor kvknumer = {inschrijving.KvkNummer}, naam = {inschrijving.InschrijvingNaam}, aantal vestigingen = {inschrijving.Vestigingen.Count}";
                            logger.Info(msg);
                            logger.Info(exportResult.Message);
                        }
                        else
                        {
                            msg =
                                $"Er ging wat mis bij het updaten van de export record voor KVK register database KVK nummer {inschrijvingCsvRecord.kvknummer}";
                            logger.Warn(msg);
                            logger.Warn(exportResult.Message);
                            logger.Warn(string.Join("\n", exportResult.Errors));
                        }
                    }
                    else
                    {
                        msg =
                            $"Er ging wat mis bij het updaten van de export record voor KVK register database KVK nummer {inschrijvingCsvRecord.kvknummer}";
                        logger.Warn(msg);
                    }
                }

                progress += incrWith;
                errorCount = loopcount - succesCount;

                RaiseRecordProcessedEvent(succesCount, errorCount, (int)progress, (int)succesprogress, msg, totalNew, totalUpdated, totalAlreadyExisted);
                //if (exportResult != null)
                //{
                //    RaiseRecordProcessedEvent(succesCount, errorCount, (int)progress, (int)succesprogress, inschrijving.Naam, totalNew, totalUpdated, totalAlreadyExisted);
                //}
                //else
                //{
                //    RaiseRecordProcessedEvent(succesCount, errorCount, (int)progress, (int)succesprogress, string.Format("Fout voor KVK nummer {0}.", inschrijvingCsvRecord.kvknummer), totalNew, totalUpdated, totalAlreadyExisted);
                //}
            }

            // Now check if we need to insert any items
            logger.Info("Found {0} items to insert in " + crmName + ".", itemsToInsert.Count());
            if (itemsToInsert.Any())
            {
                OnRecordsToInsertFound(itemsToInsert);
            }

            // And were ready... So report 100% complete to force client to finish
            errorCount = loopcount - succesCount;
            RaiseRecordProcessedEvent(succesCount, errorCount, (int)progress, (int)succesprogress, "Klaar", totalNew, totalUpdated, totalAlreadyExisted);

            
        }

        public void InsertRecords(IEnumerable<InschrijvingRecord> inschrijvingRecords)
        { 
             double incrWith = 100.0 / inschrijvingRecords.Count();

            double progress = 0;
            double succesprogress = 0;

            int loopcount = 0;
            int succesCount = 0;
            int errorCount = 0;
            int totalNew = 0;
            int totalUpdated = 0;
            int totalAlreadyExisted = 0;

            var itemsToInsert = new List<string>();
            foreach (var inschrijvingCsvRecord in inschrijvingRecords)
            {
                loopcount += 1;

                var inschrijving = _repo.GetLatestInschrijving(inschrijvingCsvRecord.kvknummer);
                IExportResult exportResult = null;
                string msg = "Nothing happend.";
                if (inschrijving == null)
                {
                    msg = $"No inschrijving exists in kernregister for kvkNummer: {inschrijvingCsvRecord.kvknummer}";
                    logger.Warn(msg);
                }
                else
                {
                    try
                    {
                        exportResult = _exportService.InsertExternalRecord(inschrijving);
                    }
                    catch (Exception ex)
                    {
                        msg =
                            $"Er ging wat mis bij het inserten van de export record voor KVK register database KVK nummer {inschrijvingCsvRecord.kvknummer}";
                        logger.Warn(msg);
                        logger.ErrorException(
                            $"Error while inserting KVKNummer = {inschrijving.KvkNummer} Naam = {inschrijving.InschrijvingNaam} Check the exception", ex); // inschrijving.Naam
                    }

                    if (exportResult != null)
                    {
                        if (exportResult.Succes)
                        {
                            succesCount++;
                            succesprogress += incrWith;
                            totalUpdated++;
                            msg =
                                $"Succesfully inserted export record voor kvknumer = {inschrijving.KvkNummer}, naam = {inschrijving.InschrijvingNaam}, aantal vestigingen = {inschrijving.Vestigingen.Count}"; //inschrijving.Naam
                            logger.Info(msg);
                        }
                        else
                        {
                            msg =
                                $"Er ging wat mis bij het inserten van de export record voor KVK register database KVK nummer {inschrijvingCsvRecord.kvknummer}";
                            logger.Warn(msg);
                            logger.Warn(exportResult.Message);
                            if(exportResult.Errors != null)
                                logger.Warn(string.Join("\n", exportResult.Errors));
                        }
                    }
                    else
                    {
                        msg =
                            $"Er ging wat mis bij het inserten van de export record voor KVK register database KVK nummer {inschrijvingCsvRecord.kvknummer}";
                        logger.Warn(msg);
                    }
                }

                progress += incrWith;
                errorCount = loopcount - succesCount;

                RaiseRecordProcessedEvent(succesCount, errorCount, (int)progress, (int)succesprogress, msg, totalNew, totalUpdated, totalAlreadyExisted);
            }

            // And were ready... So report 100% complete to force client to finish
            errorCount = loopcount - succesCount;
            RaiseRecordProcessedEvent(succesCount, errorCount, (int)progress, (int)succesprogress, "Klaar", totalNew, totalUpdated, totalAlreadyExisted);
        }

        private void RaiseRecordProcessedEvent(int succesCount, int errorCount, int progress, int successprogress, string inschrijvingsNaam, int totalNew, int totalUpdated, int totalAlreadyExisted)
        {
            var eventArgs = new RecordProcessedEventArgs
            {
                SuccesCount = succesCount,
                ErrorCount = errorCount,
                Progress = progress,
                SuccesProgress = successprogress,
                InschrijvingNaam = inschrijvingsNaam,
                TotalNew = totalNew,
                TotalUpdated = totalUpdated,
                TotalAlreadyExisted = totalAlreadyExisted
            };
            OnRecordProcessed(eventArgs);
        }

        protected virtual void OnRecordProcessed(RecordProcessedEventArgs e)
        {
            RecordProcessed?.Invoke(this, e);
        }

        protected virtual void OnRecordsToInsertFound(IEnumerable<string> recordsToInsert)
        {
            RecordsToInsertFound?.Invoke(this, new RecordsToInsertFoundEventArgs { RecordsToInsert = recordsToInsert });
        }

        public event EventHandler<RecordProcessedEventArgs> RecordProcessed;
        public event EventHandler<RecordsToInsertFoundEventArgs> RecordsToInsertFound;

        public class RecordsToInsertFoundEventArgs : EventArgs
        {
            public IEnumerable<string> RecordsToInsert { get; set; }
        }
    }
}
