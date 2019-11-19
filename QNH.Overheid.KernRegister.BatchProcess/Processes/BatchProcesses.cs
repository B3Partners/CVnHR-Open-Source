using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using QNH.Overheid.KernRegister.Business.Crm;
using QNH.Overheid.KernRegister.Business.Enums;
using QNH.Overheid.KernRegister.Business.Model;
using QNH.Overheid.KernRegister.Business.Model.Entities;
using QNH.Overheid.KernRegister.Business.Service;
using QNH.Overheid.KernRegister.Organization.Resources;

namespace QNH.Overheid.KernRegister.BatchProcess.Processes
{
    public class BatchProcesses
    {
        #region K argument

        public static void BatchProcessAllKernregisterInschrijvingen(Logger _functionalLoggerKernregistratie, Logger _logger, int MaxDegreeOfParallelism)
        {
            Console.WriteLine("Starting batch to update all " + Default.ApplicationName + " inschrijvingen.");

            _functionalLoggerKernregistratie.Debug("Start batchupdate van alle " + Default.ApplicationName + " inschrijvingen.");

            List<string> allCurrentInschrijvingen;
            using (var repo = IocConfig.Container.GetInstance<IKvkInschrijvingRepository>())
            {

                allCurrentInschrijvingen = repo.Query()
                                                    .Where(k => k.GeldigTot > DateTime.Now)
                                                    .Select(k => k.KvkNummer)
                                                    .Distinct()
                                                    .ToList();
            }

            _functionalLoggerKernregistratie.Debug("Aantal gevonden inschrijvingen: {0}", allCurrentInschrijvingen.Count());

            var searchService = IocConfig.Container.GetInstance<IKvkSearchService>();
            var syncResult = new Dictionary<KvkInschrijving, AddInschrijvingResultStatus>();
            var notFound = new List<string>();
            var errors = new List<string>();

            try
            {
                // Use ConcurrentQueue to enable safe enqueueing from multiple threads. 
                var exceptions = new ConcurrentQueue<Exception>();

                Action<string> syncMethod = (kvkNummer) =>
                {
                    try
                    {
                        var inschrijving = searchService.SearchInschrijvingByKvkNummer(kvkNummer, "Batchprocess CVnHR");
                        if (inschrijving != null)
                        {
                            var syncService = IocConfig.Container.GetInstance<IInschrijvingSyncService>();
                            var resultStatus = syncService.AddNewInschrijvingIfDataChanged(inschrijving);
                            syncResult.Add(inschrijving, resultStatus);
                        }
                        else
                        {
                            notFound.Add(kvkNummer);
                            _functionalLoggerKernregistratie.Debug("Inschrijving niet gevonden in het handelsregister: {0}", kvkNummer);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(kvkNummer);
                        exceptions.Enqueue(ex);

                        // Log on the fly
                        _logger.Error(ex);
                        //throw;
                    }
                };

                // Parallel foreach all currentInschrijvingen and catch the result
                var result = Parallel.ForEach(allCurrentInschrijvingen, new ParallelOptions() { MaxDegreeOfParallelism = MaxDegreeOfParallelism }, syncMethod);

                // round two, clear errors and exceptions and retry
                var errorsForRoundTwo = errors.ToList();
                errors.Clear();
                exceptions = new ConcurrentQueue<Exception>();
                result = Parallel.ForEach(errorsForRoundTwo, new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism }, syncMethod);

                if (exceptions.Any())
                    throw new AggregateException(exceptions);
            }
            catch (AggregateException ex)
            {
                var flatEx = ex.Flatten();

                // By throwing the error below we catch it in the main method
                //_logger.ErrorException("Error(s) while synchronizing.", flatEx);

                _functionalLoggerKernregistratie.Error(
                    $"Er zijn {ex.InnerExceptions.Count()} fout(en) opgetreden tijdens de synchronisatie:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
                //functionalLoggerKernregistratie.ErrorException("Error: ", flatEx);

                _logger.Debug("Number of items succesfully synchronized with handelsregister: " + syncResult.Count());
                throw flatEx;
            }
            finally
            {
                var msg = $@"
Aantal niet gevonden inschrijvingen: {notFound.Count()}
Aantal nieuwe inschrijvingen: {syncResult.Count(s => s.Value == AddInschrijvingResultStatus.NewInschrijvingAdded)}
Aantal geüpdatete inschrijvingen: {syncResult.Count(s => s.Value == AddInschrijvingResultStatus.InschrijvingUpdated)}
Aantal reeds bestaande inschrijvingen: {syncResult.Count(s => s.Value == AddInschrijvingResultStatus.InschrijvingAlreadyExists)}
Totaal: {syncResult.Count()}
BatchProces {(errors.Any() ? "met fouten" : "succesvol")} afgerond!

Succesvol afgerond!

";

                _functionalLoggerKernregistratie.Debug(msg);
                _logger.Debug(msg);
                _logger.Debug("Synchronized {0} " + Default.ApplicationName + " inschrijvingen.", syncResult.Count());
                Console.WriteLine("Finished updating {0} " + Default.ApplicationName + " inschrijvingen.", syncResult.Count());
            }

        }

        #endregion

        #region B argument

        public static void BatchProcessAllCrmVestigingen(Logger _functionalLoggerCrm, Logger _logger, int MaxDegreeOfParallelismCrm)
        {
            Console.WriteLine("Starting batch to update all Crm Vestigingen.");
            _functionalLoggerCrm.Debug("Start batchupdate van alle Crm vestigingen.");

            List<Vestiging> allVestigingen;
            using (var repo = IocConfig.Container.GetInstance<IKvkInschrijvingRepository>()) {
                allVestigingen = repo.GetAllCurrentVestigingen().ToList();
            }
                

            // Do a check for duplicates and remove all invalid duplicates
            _logger.Debug("Checking for duplicates");
            var duplicates = allVestigingen.Where(v => v.GeldigTot > DateTime.Now).GroupBy(v => v.Vestigingsnummer).Where(v => v.Count() > 1).ToList();
            if (duplicates.Any())
            {
                // Now check if all inschrijvingen for this duplicate still valid based on RegistratieDatumEinde, otherwise do not use in batch
                var invalidDuplicates = duplicates.SelectMany(dup =>
                        dup.Where(v =>
                                DateTime.ParseExact(v.RegistratieDatumEinde ?? "99991231", "yyyyMMdd", CultureInfo.InvariantCulture) <= DateTime.Now
                                || v.KvkInschrijving == null
                                || DateTime.ParseExact(v.KvkInschrijving.RegistratieDatumEinde ?? "99991231", "yyyyMMdd", CultureInfo.InvariantCulture) <= DateTime.Now
                                )
                            );
                allVestigingen = allVestigingen.Except(invalidDuplicates).ToList();
            }

            // Check again, if any left we have a problem
            duplicates = allVestigingen.Where(v => v.GeldigTot > DateTime.Now).GroupBy(v => v.Vestigingsnummer).Where(v => v.Count() > 1).ToList();
            if (duplicates.Any())
            {
                _logger.Debug("{0} duplicates found.", duplicates.Count());
                _functionalLoggerCrm.Error("Dubbele vestigingen gevonden in de " + Default.ApplicationName + " database. Neem contact op met de systeembeheerder!");

                var msgDuplicates = "Duplicate VestigingId's: " + Environment.NewLine + string.Join(Environment.NewLine, duplicates.Select(d => d.Key)) + Environment.NewLine;
                _logger.Debug(msgDuplicates);
                _functionalLoggerCrm.Debug(msgDuplicates);
                throw new InvalidDataException("Duplicates found!");
            }
            else
                _logger.Debug("No duplicates found.");


            _functionalLoggerCrm.Debug("{0} vestigingen gevonden in de " + Default.ApplicationName, allVestigingen.Count());

            try
            {
                var exportService = IocConfig.Container.GetInstance<IExportService>();
                exportService.UpdateAllExistingExternalVestigingen(allVestigingen, _functionalLoggerCrm, MaxDegreeOfParallelismCrm);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error(s) while synchronizing Crm.");
                _functionalLoggerCrm.Error(ex, "Error(s) opgetreden tijdens de Crm batchUpdate!");
                _functionalLoggerCrm.Error(Environment.NewLine);
                throw;
            }

            _functionalLoggerCrm.Debug("Succesvol afgerond!" + Environment.NewLine);
            Console.WriteLine("Finished updating all Crm vestigingen.");
        }

        #endregion
    }
}
