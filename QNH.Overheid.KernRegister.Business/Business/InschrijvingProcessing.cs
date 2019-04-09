using NLog;
using QNH.Overheid.KernRegister.Business.Enums;
using QNH.Overheid.KernRegister.Business.Model;
using QNH.Overheid.KernRegister.Business.Model.Entities;
using QNH.Overheid.KernRegister.Business.Service;
using QNH.Overheid.KernRegister.Business.Service.BRMO;
using QNH.Overheid.KernRegister.Business.Utility;
using StructureMap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QNH.Overheid.KernRegister.Business.Business
{
    public enum ProcessTaskTypes
    {
        CVnHR,
        Brmo
    }

    public class InschrijvingProcessing
    {
        private readonly int _maxDegreeOfParallelism;
        private readonly IContainer _container;

        private readonly Logger _logger;

        public InschrijvingProcessing(IContainer container, int maxDegreeOfParallelism, Logger logger = null)
        {
            _container = container;
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
            _logger = logger ?? LogManager.GetCurrentClassLogger();
        }

        public void ProcessRecords(IEnumerable<InschrijvingRecord> inschrijvingRecords, string userName, ProcessTaskTypes type)
        {
            // Ensure unique records
            inschrijvingRecords = inschrijvingRecords.Distinct();

            // This should be refactored, or not?!
            ProcessInschrijvingen(inschrijvingRecords, 
                (kvkNummer) => {
                    var searchService = _container.GetInstance<IKvkSearchService>();
                    switch (type)
                    {
                        case ProcessTaskTypes.CVnHR:
                            return searchService.SearchInschrijvingByKvkNummer(kvkNummer, userName);
                        case ProcessTaskTypes.Brmo:
                            return RawXmlCache.Get(kvkNummer, () => { searchService.DoeOpvragingBijKvk(kvkNummer, userName); });
                        default:
                            throw new NotImplementedException($"Could not process type: ${type}");
                    }
                },
                (kvkInschrijving) => {
                    switch (type)
                    {
                        case ProcessTaskTypes.CVnHR:
                            var repo = _container.GetInstance<IKvkInschrijvingRepository>();
                            var service = new InschrijvingSyncService(repo);
                            return service.AddNewInschrijvingIfDataChanged((KvkInschrijving)kvkInschrijving);
                        case ProcessTaskTypes.Brmo:
                            var brmoSyncService = _container.GetInstance<IBrmoSyncService>();
                            return brmoSyncService.UploadXDocumentToBrmo((XDocument)kvkInschrijving);
                        default:
                            throw new NotImplementedException($"Could not process type: ${type}");
                    }
                },
                type);
        }

        private void ProcessInschrijvingen(IEnumerable<InschrijvingRecord> inschrijvingRecords, 
            Func<string, object> retrievalFunc, 
            Func<object, AddInschrijvingResultStatus> storageFunc,
            ProcessTaskTypes type)
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
            ConcurrentBag<string> errors = new ConcurrentBag<string>();

            //foreach (var inschrijvingCsvRecord in inschrijvingRecords)
            Parallel.ForEach(inschrijvingRecords, new ParallelOptions() { MaxDegreeOfParallelism = _maxDegreeOfParallelism }, (inschrijvingCsvRecord) =>
            {
                bool success = false;
                loopcount += 1;
                object retrievalResult = null;
                try
                {
                    retrievalResult = retrievalFunc.Invoke(inschrijvingCsvRecord.kvknummer);
                }
                catch (Exception ex)
                {
                    // Just log the Exception and continue
                    _logger.Error("Er ging wat mis bij het ophalen van gegevens voor KVK nummer {0}", inschrijvingCsvRecord.kvknummer);
                    _logger.Error(ex, $"Error while retrieving KVKNummer = {inschrijvingCsvRecord.kvknummer} Check the exception");
                }
                var inschrijvingNaam = "[nvt]";
                var aantalVestigingen = 0;
                if (retrievalResult != null)
                {
                    switch (type)
                    {
                        case ProcessTaskTypes.CVnHR:
                            var kvkInschrijving = (KvkInschrijving)retrievalResult;
                            inschrijvingNaam = kvkInschrijving.InschrijvingNaam;
                            aantalVestigingen = kvkInschrijving.Vestigingen.Count;
                            break;
                    }
                }
                else
                {
                    _logger.Warn("Inschrijving met KVK nummer {0} niet gevonden. Mogelijke oorzaak, nummer is niet bekend bij KVK.", inschrijvingCsvRecord.kvknummer);
                }

                _logger.Debug(
                        $"Storing KvkInschrijving with kvknummer = {inschrijvingCsvRecord.kvknummer}, naam = {inschrijvingNaam}");
                AddInschrijvingResultStatus status;
                try
                {
                    status = storageFunc.Invoke(retrievalResult);
                    switch (status)
                    {
                        case AddInschrijvingResultStatus.NewInschrijvingAdded:
                            _logger.Debug(
                                $"Nieuwe inschrijving opgeslagen voor kvknumer = {inschrijvingCsvRecord.kvknummer}, naam = {inschrijvingNaam}, aantal vestigingen = {aantalVestigingen}");
                            totalNew += 1;
                            break;
                        case AddInschrijvingResultStatus.InschrijvingUpdated:
                            _logger.Debug(
                                $"Inschrijving bijgewerkt voor kvknumer = {inschrijvingCsvRecord.kvknummer}, naam = {inschrijvingNaam}, aantal vestigingen = {aantalVestigingen}");
                            totalUpdated += 1;
                            break;
                        case AddInschrijvingResultStatus.InschrijvingAlreadyExists:
                            _logger.Debug(
                                $"Inschrijving bestond al voor kvknumer = {inschrijvingCsvRecord.kvknummer}, naam = {inschrijvingNaam}, aantal vestigingen = {aantalVestigingen}");
                            totalAlreadyExisted += 1;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Did not receive a status...");
                    }

                    succesprogress += incrWith;
                    succesCount += 1;
                    success = true;
                }
                catch (Exception ex)
                {
                    _logger.Error("Er ging wat mis bij het opslaan van gegevens in KVK register database KVK nummer {0}", inschrijvingCsvRecord.kvknummer);
                    _logger.Error(ex,
                        $"Error while storing KVKNummer = {inschrijvingCsvRecord.kvknummer} Naam = {inschrijvingNaam} Check the exception");
                    errors.Add(inschrijvingCsvRecord.kvknummer);
                }


                progress += incrWith;
                errorCount = loopcount - succesCount;

                if (retrievalResult != null)
                {
                    RaiseRecordProcessedEvent(succesCount, errorCount, (int)progress, (int)succesprogress, success ? inschrijvingNaam : "Fout voor " + inschrijvingCsvRecord.kvknummer + ": " + inschrijvingNaam, totalNew, totalUpdated, totalAlreadyExisted, type, !success, inschrijvingCsvRecord.kvknummer, null);
                }
                else
                {
                    RaiseRecordProcessedEvent(succesCount, errorCount, (int)progress, (int)succesprogress,
                        $"Fout voor KVK nummer {inschrijvingCsvRecord.kvknummer}", totalNew, totalUpdated, totalAlreadyExisted,type, true, inschrijvingCsvRecord.kvknummer, null);
                }
            });

            // And were ready... So report 100% complete to force client to finish
            // Clients.All.reportProgress(100);
            errorCount = loopcount - succesCount;
            RaiseRecordProcessedEvent(succesCount, errorCount,(int) progress, (int)succesprogress, "Klaar",totalNew, totalUpdated, totalAlreadyExisted, type, false, "0", errors.ToList());
        }

        private void RaiseRecordProcessedEvent(int succesCount, int errorCount, int progress, int successprogress, string inschrijvingsNaam, int totalNew, int totalUpdated, int totalAlreadyExisted, ProcessTaskTypes type, bool isError, string kvkNummer, List<string> errors)
        {
            var eventArgs = new RecordProcessedEventArgs
            {
                Type = type,
                SuccesCount = succesCount,
                ErrorCount = errorCount,
                Progress = progress,
                SuccesProgress = successprogress,
                InschrijvingNaam = inschrijvingsNaam,
                TotalNew = totalNew,
                TotalUpdated = totalUpdated,
                TotalAlreadyExisted = totalAlreadyExisted,
                IsError = isError,
                KvkNummer = kvkNummer,
                Errors = errors
            };
            OnRecordProcessed(eventArgs);
        }

        protected virtual void OnRecordProcessed(RecordProcessedEventArgs e)
        {
            RecordProcessed?.Invoke(this, e);
        }

        public event EventHandler<RecordProcessedEventArgs> RecordProcessed;
    }
}