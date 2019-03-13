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
                Log(brmoLogger, "Could not start BRMO proces. Mandatory arguments missing... (possibly you've tried to start an empty task? Navigate via menu item 'Taken beheren'...)", new ArgumentException("args"));
                return;
            }
            var version = args[1];
            if (string.IsNullOrWhiteSpace(version))
            {
                Log(brmoLogger, "Could not start proces. Argument version missing.", new ArgumentException("version")); ;
                return;
            }
            var type = args[2];
            if (string.IsNullOrWhiteSpace(type))
            {
                Log(brmoLogger, "Could not start proces. Argument type missing.", new ArgumentException("type")); ;
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
                    Log(brmoLogger, "Could not start proces. UploadFolder is missing.", new ArgumentException("uploadfolder is missing"));
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
                    Log(brmoLogger, $"Could not start process. Could not find (part of) the path {fullPath}. Please check the UploadFolder configuration setting",
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
                                Log(brmoLogger, "adding prefix 0 for KVK-nummer:" + inschrijvingRecord.kvknummer);
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
                Log(brmoLogger, $"Could not start proces. No kvknumbers found for process {type}!", new ArgumentException("kvknumbers, zipcodes or csv")); ;
                return;
            }
            var zipCodeLogger = zipCodes.Count() > 20
                ? $"{zipCodes.Count().ToString()} items"
                : string.Join(" ", zipCodes);
            Log(brmoLogger, $"Starting BRMO task for version {version} with {brmoProcessType} {zipCodeLogger}");
            FillBrmoForZipcodes(maxDegreeOfParallelism, brmoLogger, version, brmoProcessType, zipCodes);
            Log(brmoLogger, "Finished BRMO task");
        }

        private static void FillBrmoForZipcodes(int maxDegreeOfParallelism, 
            Logger brmoLogger, 
            string HRDataserviceVersion,
            BrmoProcessTypes type, 
            List<string> items = null)
        {
            Console.WriteLine("Make sure to run this process as administrator!");

            List<string> kvkIds;
            if (type == BrmoProcessTypes.ZipCodes)
            {
                var msg = $"Searching kvkIds in zipcodes: {string.Join(" ", items)}";
                brmoLogger.Debug(msg);
                Console.WriteLine(msg);
                kvkIds = ZipcodeProcesses.GetKvkIdsForZipcode(maxDegreeOfParallelism, true, items.ToArray()).ToList();
                msg = $"Found {kvkIds.Count()} kvk Ids.";
                brmoLogger.Debug(msg);
                Console.WriteLine(msg);
            }
            else
                kvkIds = items;

            Log(brmoLogger, $"Found {kvkIds.Count()} items");
            kvkIds = kvkIds.Distinct().ToList();
            Log(brmoLogger, $"Found {kvkIds.Count()} unique items");

            var service = IocConfig.Container.GetInstance<IKvkSearchService>();
            var brmoSyncService = IocConfig.Container.GetInstance<IBrmoSyncService>();

            var errors = new List<Exception>();
            var errorKvkNummers = new List<string>();

            Parallel.ForEach(kvkIds, new ParallelOptions() { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (kvkNummer) =>
            {
                try
                {
                    // retry without bypassing cache
                    var xDoc = RawXmlCache.Get(kvkNummer, () => { service.DoeOpvragingBijKvk(kvkNummer, "Batchprocess BRMO"); });
                    var status = brmoSyncService.UploadXDocumentToBrmo(xDoc);
                    if (status != AddInschrijvingResultStatus.BrmoInschrijvingCreated)
                    {
                        throw new Exception("Status not expected: " + status.ToString());
                    }
                }
                catch (Exception ex)
                {
                    brmoLogger.Error(ex, "Exception for kvkNummer: " + kvkNummer);
                    errors.Add(ex);
                    errorKvkNummers.Add(kvkNummer);
                    Console.Write($"\r{ex.Message}");
                }
            });

            brmoLogger.Info($"Succesfully uploaded {kvkIds.Count() - errors.Count()} kvk Ids. Found {errors.Count()} errors...");
            brmoLogger.Info($@"Kvknummers to retry:
{string.Join(Environment.NewLine, errorKvkNummers)}");
            Console.WriteLine();
        }

        private static void Log(Logger brmoLogger, string msg, Exception ex = null)
        {
            Console.WriteLine(msg);
            if (ex == null)
                brmoLogger.Info(msg);
            else
                brmoLogger.Error(ex, msg);
        }
    }
}
