using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using QNH.Overheid.KernRegister.Business.Business;
using QNH.Overheid.KernRegister.Business.Enums;
using QNH.Overheid.KernRegister.Business.Service;
using QNH.Overheid.KernRegister.Business.Service.BRMO;
using QNH.Overheid.KernRegister.Business.Utility;

namespace QNH.Overheid.KernRegister.BatchProcess.Processes
{
    public class BrmoProcesses
    {
        public static void BatchProcessBrmo(string[] args,
            int maxDegreeOfParallelism,
            Logger brmoLogger)
        {
            if (args.Count() == 1)
            {
                log(brmoLogger, "Could not start BRMO proces. Mandatory arguments missing... (possibly you've tried to start an empty task? Navigate via menu item 'Taken beheren'...)", new ArgumentException("args"));
                return;
            }
            var version = args[1];
            if (string.IsNullOrWhiteSpace(version))
            {
                log(brmoLogger, "Could not start proces. Argument version missing.", new ArgumentException("version")); ;
                return;
            }
            var type = args[2];
            if (string.IsNullOrWhiteSpace(type))
            {
                log(brmoLogger, "Could not start proces. Argument type missing.", new ArgumentException("type")); ;
                return;
            }

            var brmoProcessType = BrmoProcessTypes.ZipCodes;
            switch (type)
            {
                case "KvkIds":
                    brmoProcessType = BrmoProcessTypes.KvkIds;
                    break;
                case "ZipCodes":
                    brmoProcessType = BrmoProcessTypes.ZipCodes;
                    break;
                case "Csv":
                    brmoProcessType = BrmoProcessTypes.Csv;
                    break;
            }
            var zipCodes = new List<string>();
            if (brmoProcessType == BrmoProcessTypes.Csv)
            {
                var uploadFolder = ConfigurationManager.AppSettings["uploadFolder"];
                if (string.IsNullOrWhiteSpace(uploadFolder))
                {
                    log(brmoLogger, "Could not start proces. UploadFolder is missing.", new ArgumentException("uploadfolder is missing"));
                }

                var prefix = "";
                var usePrefixZero = Convert.ToBoolean(ConfigurationManager.AppSettings["UseZeroPrefix"] ?? "false");
                if (usePrefixZero)
                {
                    prefix = "0";
                }

                var path = args[3];
                var fullPath = Path.Combine(uploadFolder, path);
                //determine whether path is absolute or relative
                if (!fullPath.Contains(":"))
                {
                    fullPath = Path.Combine(
                            Directory.GetParent(
                                Directory.GetParent(
                                    AppDomain.CurrentDomain.BaseDirectory
                                    ).ToString()
                                 ).ToString()
                                 , fullPath);
                }
                if (!File.Exists(fullPath))
                {
                    log(brmoLogger, $"Could not start process. Could not find (part of) the path {fullPath}. Please check the UploadFolder configuration setting",
                        new ArgumentException("Could not find path"));
                    return;
                }

                var kvkNummers = CsvUtils.ReadInschrijvingRecords(fullPath);
                if (kvkNummers.Any())
                {
                    if (usePrefixZero)
                    {
                        foreach (var inschrijvingRecord in kvkNummers)
                        {
                            if (inschrijvingRecord.kvknummer.Length == 7)
                            {
                                log(brmoLogger, "adding prefix 0 for KVK-nummer:" + inschrijvingRecord.kvknummer, null);
                                zipCodes.Add(prefix + inschrijvingRecord.kvknummer);
                            }
                        }
                    }
                    else
                    {
                        zipCodes = kvkNummers.Select(k => k.kvknummer).ToList();
                    }
                }
                else
                {
                    brmoProcessType = BrmoProcessTypes.ZipCodes;
                    prefix = "";
                }
            }
            else
            {
                zipCodes = args.Skip(3).ToList();
            }
            if (!zipCodes.Any())
            {
                log(brmoLogger, $"Could not start proces. No kvknumbers found for process {type}!", new ArgumentException("kvknumbers, zipcodes or csv")); ;
                return;
            }
            log(brmoLogger, $"Starting BRMO task for version {version} with {brmoProcessType} {string.Join(" ", zipCodes)}", null);
            FillBrmoForZipcodes(maxDegreeOfParallelism, brmoLogger, version, brmoProcessType, zipCodes);
            log(brmoLogger, "Finished BRMO task", null);
        }

        private static void FillBrmoForZipcodes(int maxDegreeOfParallelism, 
            Logger log, 
            string HRDataserviceVersion,
            BrmoProcessTypes type, 
            List<string> items = null)
        {
            Console.WriteLine("Make sure to run this process as administrator!");

            List<string> kvkIds;
            if (type == BrmoProcessTypes.ZipCodes)
            {
                var msg = $"Searching kvkIds in zipcodes: {string.Join(" ", items)}";
                log.Debug(msg);
                Console.WriteLine(msg);
                kvkIds = ZipcodeProcesses.GetKvkIdsForZipcode(maxDegreeOfParallelism, true, items.ToArray()).ToList();
                msg = $"Found {kvkIds.Count()} kvk Ids.";
                log.Debug(msg);
                Console.WriteLine(msg);
            }
            else
                kvkIds = items;

            var service = IocConfig.Container.GetInstance<IKvkSearchService>();
            var brmoSyncService = IocConfig.Container.GetInstance<IBrmoSyncService>();

            var errors = new List<Exception>();
            var errorKvkNummers = new List<string>();

            Parallel.ForEach(kvkIds, new ParallelOptions() { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (kvkNummer) =>
            {
                try
                {
                    // retry without bypassing cache
                    var xDoc = RawXmlCache.Get(kvkNummer, () => { service.SearchInschrijvingByKvkNummer(kvkNummer, "Batchprocess BRMO"); });
                    var status = brmoSyncService.UploadXDocumentToBrmo(xDoc);
                    if (status != AddInschrijvingResultStatus.BrmoInschrijvingCreated)
                    {
                        throw new Exception("Status not expected: " + status.ToString());
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Exception for kvkNummer: " + kvkNummer);
                    errors.Add(ex);
                    errorKvkNummers.Add(kvkNummer);
                    Console.Write($"\r{ex.Message}");
                }
            });

            log.Info($"Succesfully uploaded {kvkIds.Count() - errors.Count()} kvk Ids. Found {errors.Count()} errors...");
            log.Info($@"Kvknummers to retry:
{string.Join(Environment.NewLine, errorKvkNummers)}");
            Console.WriteLine();
        }

        private static void log(Logger brmoLogger, string msg, Exception ex)
        {
            Console.WriteLine(msg);
            if (ex == null)
                brmoLogger.Info(msg);
            else
                brmoLogger.Error(ex, msg);
        }
    }
}
