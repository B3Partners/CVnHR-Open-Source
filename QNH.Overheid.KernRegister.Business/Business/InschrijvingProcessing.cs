using NLog;
using QNH.Overheid.KernRegister.Business.Enums;
using QNH.Overheid.KernRegister.Business.Model;
using QNH.Overheid.KernRegister.Business.Model.Entities;
using QNH.Overheid.KernRegister.Business.Service;
using QNH.Overheid.KernRegister.Business.Utility;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.Business.Business
{
    public class InschrijvingProcessing
    {
        private readonly int _maxDegreeOfParallelism;
        private readonly IContainer _container;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public InschrijvingProcessing(IContainer container, int maxDegreeOfParallelism)
        {
            _container = container;
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        public void ProcessRecords(IEnumerable<InschrijvingRecord> inschrijvingRecords, string userName)
        {
            // Ensure unique records
            inschrijvingRecords = inschrijvingRecords.Distinct();

            // This should be refactored, or not?!
            ProcessInschrijvingen(inschrijvingRecords, 
                (kvkNummer) => {
                    var searchService = _container.GetInstance<IKvkSearchService>();
                    return searchService.SearchInschrijvingByKvkNummer(kvkNummer, userName);
                },
                (kvkInschrijving) => {
                    var repo = _container.GetInstance<IKvkInschrijvingRepository>();
                    var service = new InschrijvingSyncService(repo);
                    var status = service.AddNewInschrijvingIfDataChanged(kvkInschrijving);

                    return status;
                });
        }

        private void ProcessInschrijvingen(IEnumerable<InschrijvingRecord> inschrijvingRecords, Func<string, KvkInschrijving> retrievalFunc, Func<KvkInschrijving, AddInschrijvingResultStatus> storageFunc)
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

            //foreach (var inschrijvingCsvRecord in inschrijvingRecords)
            Parallel.ForEach(inschrijvingRecords, new ParallelOptions() { MaxDegreeOfParallelism = _maxDegreeOfParallelism }, (inschrijvingCsvRecord) =>
            {
                bool success = false;
                loopcount += 1;
                KvkInschrijving kvkInschrijving = null;
                try
                {
                    kvkInschrijving = retrievalFunc.Invoke(inschrijvingCsvRecord.kvknummer);
                }
                catch (Exception ex)
                {
                    // Just log the Exception and continue
                    logger.Error("Er ging wat mis bij het ophalen van gegevens voor KVK nummer {0}", inschrijvingCsvRecord.kvknummer);
                    logger.Error(ex, $"Error while retrieving KVKNummer = {inschrijvingCsvRecord.kvknummer} Check the exception");

                }
                if (kvkInschrijving != null)
                {
                    logger.Debug(
                        $"Storing KvkInschrijving with kvknummer = {kvkInschrijving.KvkNummer}, naam = {kvkInschrijving.InschrijvingNaam}");
                    AddInschrijvingResultStatus status;
                    try
                    {
                        status = storageFunc.Invoke(kvkInschrijving);
                        switch (status)
                        {
                            case AddInschrijvingResultStatus.NewInschrijvingAdded:
                                logger.Info(
                                    $"Nieuwe inschrijving opgeslagen voor kvknumer = {kvkInschrijving.KvkNummer}, naam = {kvkInschrijving.InschrijvingNaam}, aantal vestigingen = {kvkInschrijving.Vestigingen.Count}");
                                totalNew += 1;
                                break;
                            case AddInschrijvingResultStatus.InschrijvingUpdated:
                                logger.Info(
                                    $"Inschrijving bijgewerkt voor kvknumer = {kvkInschrijving.KvkNummer}, naam = {kvkInschrijving.InschrijvingNaam}, aantal vestigingen = {kvkInschrijving.Vestigingen.Count}");
                                totalUpdated += 1;
                                break;
                            case AddInschrijvingResultStatus.InschrijvingAlreadyExists:
                                logger.Info(
                                    $"Inschrijving bestond al voor kvknumer = {kvkInschrijving.KvkNummer}, naam = {kvkInschrijving.InschrijvingNaam}, aantal vestigingen = {kvkInschrijving.Vestigingen.Count}");
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
                        logger.Error("Er ging wat mis bij het opslaan van gegevens in KVK register database KVK nummer {0}", inschrijvingCsvRecord.kvknummer);
                        logger.Error(ex,
                            $"Error while storing KVKNummer = {kvkInschrijving.KvkNummer} Naam = {kvkInschrijving.InschrijvingNaam} Check the exception");
                    }
                }
                else
                {
                    logger.Warn("Inschrijving met KVK nummer {0} niet gevonden. Mogelijke oorzaak, nummer is niet bekend bij KVK.", inschrijvingCsvRecord.kvknummer);
                }

                progress += incrWith;
                errorCount = loopcount - succesCount;

                if (kvkInschrijving != null)
                {
                    RaiseRecordProcessedEvent(succesCount, errorCount, (int)progress, (int)succesprogress, success ? kvkInschrijving.InschrijvingNaam : "Fout voor " + kvkInschrijving.KvkNummer + ": " + kvkInschrijving.InschrijvingNaam, totalNew, totalUpdated, totalAlreadyExisted);
                }
                else
                {
                    RaiseRecordProcessedEvent(succesCount, errorCount, (int)progress, (int)succesprogress,
                        $"Fout voor KVK nummer {inschrijvingCsvRecord.kvknummer}", totalNew, totalUpdated, totalAlreadyExisted);
                }
            });

            // And were ready... So report 100% complete to force client to finish
            // Clients.All.reportProgress(100);
            errorCount = loopcount - succesCount;
            RaiseRecordProcessedEvent(succesCount, errorCount,(int) progress, (int)succesprogress, "Klaar",totalNew, totalUpdated, totalAlreadyExisted);

        }

        private void RaiseRecordProcessedEvent(int succesCount, int errorCount,   int progress, int successprogress, string inschrijvingsNaam, int totalNew, int totalUpdated, int totalAlreadyExisted)
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

        public event EventHandler<RecordProcessedEventArgs> RecordProcessed;
    }
}