using CsvHelper;
using NLog;
using QNH.Overheid.KernRegister.BatchProcess.Processes;
using QNH.Overheid.KernRegister.Business.Business;
using QNH.Overheid.KernRegister.Business.Crm;
using QNH.Overheid.KernRegister.Business.Enums;
using QNH.Overheid.KernRegister.Business.KvK;
using QNH.Overheid.KernRegister.Business.Model;
using QNH.Overheid.KernRegister.Business.Model.Entities;
using QNH.Overheid.KernRegister.Organization.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace QNH.Overheid.KernRegister.BatchProcess
{
    public partial class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        private static readonly Logger _functionalLoggerKernregistratie = LogManager.GetLogger("functionalLoggerKernregistratie");
        private static readonly Logger _functionalLoggerCrm = LogManager.GetLogger("functionalLoggerCrm");
        private static readonly Logger _brmoLogger = LogManager.GetLogger("brmoLogger");

        private static Dataservice _service;

        private static int? _maxDegreeOfParallelism;
        private static int MaxDegreeOfParallelism
        { 
            get
            {
                if (!_maxDegreeOfParallelism.HasValue)
                    _maxDegreeOfParallelism = Convert.ToInt32(ConfigurationManager.AppSettings["MaxDegreeOfParallelism"] ?? "1");
                return _maxDegreeOfParallelism.Value;
            }
        }

        private static int? _maxDegreeOfParallelismCrm;
        private static int MaxDegreeOfParallelismCrm
        {
            get
            {
                if (!_maxDegreeOfParallelismCrm.HasValue)
                    _maxDegreeOfParallelismCrm = Convert.ToInt32(ConfigurationManager.AppSettings["MaxDegreeOfParallelismCrm"] ?? "1");
                return _maxDegreeOfParallelismCrm.Value;
            }
        }

        static void Main(string[] args)
        {
            try
            {
                Structuremap.Start();

                if (args.Length == 0)
                {
                    Console.WriteLine("- Starting. Please specify an argument (? or any for help):");
                    args = Console.ReadLine()?.Split(' ', ',');
                }
                
                var arg = args?[0] ?? "?";

                switch (arg?.ToUpper())
                {
                    case "B":
                        BatchProcesses.BatchProcessAllCrmVestigingen(_functionalLoggerCrm, _logger, MaxDegreeOfParallelismCrm);
                        break;
                    case "K":
                        BatchProcesses.BatchProcessAllKernregisterInschrijvingen(_functionalLoggerKernregistratie, _logger, MaxDegreeOfParallelism);
                        break;
                    case "D":
                        DownloadAll(false);
                        break;
                    case "I":
                        ImportAll();
                        break;
                    case "V":
                        DownloadAll(true);
                        break;
                    case "Z":
                        var kvkIds = ZipcodeProcesses.GetKvkIdsForZipcode(MaxDegreeOfParallelism).ToList();
                        kvkIds.Insert(0,"kvknummer");
                        File.AppendAllLines("Csv/kvkIdsForZipcode.csv", kvkIds);
                        Console.WriteLine("Finished downloading all kvkids for zipcodes found in file. File created: Csv/kvkIdsForZipcode.csv");
                        break;
                    case "T":
                        ZipcodeProcesses.TestPostcodeService();
                        break;
                    case "S":
                        SanityCheck();
                        break;
                    case "A":
                        KvKDataserviceV2_5.Test();
                        break;
                    case "BRMO":
                        Action<string, Exception> log = (msg, ex) => {
                            Console.WriteLine(msg);
                            if (ex == null)
                                _brmoLogger.Info(msg);
                            else
                                _brmoLogger.Error(ex, msg);
                        };
                        var version = args[1];
                        if (string.IsNullOrWhiteSpace(version))
                        {
                            log("Could not start proces. Argument version missing.", new ArgumentException("version")); ;
                            break;
                        }
                        var type = args[2];
                        var brmoProcessType = BrmoProcessTypes.ZipCodes;
                        if (string.IsNullOrWhiteSpace(type))
                        {
                            log("Could not start proces. Argument version missing.", new ArgumentException("version")); ;
                            break;
                        }
                        else
                        {
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
                        }
                        var zipCodes = new List<String>();
                        if (brmoProcessType == BrmoProcessTypes.Csv)
                        {
                            var i = 0;
                            var uploadFolder = ConfigurationManager.AppSettings["uploadFolder"];
                            var path = args[3];
                            using (var reader = new StreamReader(uploadFolder + "\\" + path))
                            {
                                while (!reader.EndOfStream)
                                {
                                    
                                    var line = reader.ReadLine();
                                    if (i != 0)
                                    {
                                        zipCodes.Add(line);
                                    }
                                    i++;
                                }
                            }
                        }
                        else
                        {
                            zipCodes = args.Skip(3).ToList();
                        }
                        if (!zipCodes.Any())
                        {
                            log("Could not start proces. Zipcodes missing!", new ArgumentException("version")); ;
                            break;
                        }
                        log($"Starting BRMO task for version {version} with {brmoProcessType} {string.Join(" ", zipCodes)}", null);
                        RsgbProcesses.FillRsgbForZipcodes(MaxDegreeOfParallelism, _brmoLogger, version, brmoProcessType, zipCodes);
                        log("Finished BRMO task", null);
                        break;
                    case "PROBIS":
                        var probis = IocConfig.Container.GetInstance<IFinancialExportService>();
                        var probisResult = probis.InsertVestiging(new Vestiging() { KvkInschrijving = new KvkInschrijving()}, FinancialProcesType.ProbisDebiteuren);
                        Console.WriteLine($"Result: {probisResult.Succes} + {probisResult.Message}");
                        break;
                    default:
                        Console.WriteLine(@"
QNH.Overheid.KernRegister.BatchProcess

- Arguments:

    B => BatchProcess update all Crm vestigingen from " + Default.ApplicationName + @" vestigingen
    K => BatchProcess update all " + Default.ApplicationName + @" inschrijvingen from Handelsregister
    D => Download all kvkNumbers from any of the csv files found in the Csv directory and writes out the returned XML response in the DATA directory
    I => Imports all kvkNumbers from any of the csv files found in the Csv directory into the KernRegister database
    V => Same as D, but for vestigingNumbers
    Z => Get all KvKIds for the zipcodes from any of the csv files found in the Zipcode directory and creates a csv file in the Csv directory
    T => Test Postcode service
    S => SanityCheck
    A => Test KvKDataservice Version 2.5
    RSGB => Fill RSGB for the zipcodes provided (e.g. 7283 7705 for zipcodes 7283 and 7705, or 'drenthe' for the total of Drenthe)
");
                        Console.ReadLine();
                        break;
                }

                if (args.Length > 0)
                    Console.WriteLine("finished succesfully.");
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Finished with error(s)!", ex);
                throw;
            }
        }

        #region D and V argument

        private static int _downloadTotal = 0;
        private static int _downloadCurrent = 0;
        private static object _errorsLock = new object();
        private static List<string> _errors = new List<string>() { "kvknummer" };

        private static void DownloadAll(bool vestiging)
        {
            var records = new List<InschrijvingRecord>();
            foreach (var csvFile in Directory.GetFiles("Csv", "*.csv"))
            {
                var file = new FileInfo(csvFile);
                if (!file.Exists)
                    continue;

                if (file.Extension.ToLower() != ".csv")
                    continue;
                records.AddRange(ReadInschrijvingRecords(file.FullName).Distinct());
            }

            _downloadTotal = records.Count;
            _downloadCurrent = 0;
            Console.WriteLine("Starting to download " + _downloadTotal + " records.");

            _service = IocConfig.Container.GetInstance<Dataservice>();
            var saveDirectory = new DirectoryInfo("Data");

            var serializer = new XmlSerializer(typeof(ophalenInschrijvingResponse));

            var serviceDown = false;
            var running = true;
            new Task(() => {
                // While runnning check if any of the threads reported that the service is down
                // If so periodically check if the service is up again.
                while (running)
                {
                    if (serviceDown)
                    {
                        try
                        {
                            var response = GetProductTypeResponse("01179514");
                            serviceDown = false;
                            Console.WriteLine("Service running again!");
                        }
                        catch
                        {
                            Console.WriteLine("Service is down.");
                        }
                    }
                    Thread.Sleep(new TimeSpan(0, 1, 0));
                }
            }).Start();

            Parallel.ForEach(records, new ParallelOptions() { MaxDegreeOfParallelism = MaxDegreeOfParallelism },
                (record)=>{
                    DownloadAndSerializeKvKId(vestiging, record, saveDirectory, serializer, ref serviceDown);
                });
            
            // stop running
            running = false;

            // write out the errors
            File.AppendAllLines("Csv/DownloadErrors.csv", _errors);

            Console.WriteLine("Finished!");
        }

        private static void DownloadAndSerializeKvKId(bool vestiging, InschrijvingRecord record, DirectoryInfo saveDirectory, XmlSerializer serializer, ref bool serviceDown)
        {
            var nummer = record.kvknummer;
            var path = vestiging
                ? Path.Combine(saveDirectory.FullName, "vestiging-" + nummer + ".xml")
                : Path.Combine(saveDirectory.FullName, "kvk-" + nummer + ".xml");
            
            _downloadCurrent++;
            
            if (File.Exists(path))
                return;

            ZipcodeProcesses.ShowPercentProgress(string.Concat("Processing: ", nummer, "... (", _downloadCurrent, ") " ), _downloadCurrent, _downloadTotal);

            // If the service is running, go!
            if (!serviceDown)
            {
                try
                {
                    if (vestiging)
                    {
                        var v = GetVestigingProductTypeResponse(nummer);
                        using (var writer = new StreamWriter(path))
                            serializer.Serialize(writer, v);
                    }
                    else
                    {
                        var inschrijving = GetProductTypeResponse(nummer);
                        using (var writer = new StreamWriter(path))
                            serializer.Serialize(writer, inschrijving);
                    }

                    //Console.WriteLine(nummer); 
                    return;
                }
                catch (FaultException ex)
                {
                    _logger.ErrorException("Error for number: " + nummer, ex);
                    Console.WriteLine("Error for number: " + nummer + " -> check the log file.");
                    lock (_errorsLock)
                    {
                        _errors.Add(nummer);
                    }
                    serviceDown = true;
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("Error for number: " + nummer, ex);
                    Console.WriteLine("Error for number: " + nummer + " -> check the log file.");
                    lock (_errorsLock)
                    {
                        _errors.Add(nummer);
                    }
                }
            }

            // If service is found to be down, wait until the service is running again and try again.
            if (serviceDown)
            {
                while (serviceDown)
                {
                    Thread.Sleep(new TimeSpan(0, 1, 0));
                }
                // Service reported to be working again! Now try again!
                DownloadAndSerializeKvKId(vestiging, record, saveDirectory, serializer, ref serviceDown);
            }
        }

        private static ophalenInschrijvingResponse GetProductTypeResponse(string kvkNummer)
        {
            var result = _service.ophalenInschrijving(new ophalenInschrijvingRequest()
            {
                ophalenInschrijvingRequest1 = new InschrijvingRequestType() { 
                    klantreferentie = ConfigurationManager.AppSettings["SearchServiceKlantReferentie"],
                    kvkNummer = kvkNummer
                }
            });

            return result;
        }

        private static ophalenVestigingResponse GetVestigingProductTypeResponse(string vestigingNummer)
        {
            var result = _service.ophalenVestiging(new ophalenVestigingRequest(new VestigingRequestType()
            {
                klantreferentie = ConfigurationManager.AppSettings["SearchServiceKlantReferentie"],
                vestigingsnummer = vestigingNummer
            }));

            return result;
        }

        #endregion

        #region I argument

        private static void ImportAll()
        {
            Console.WriteLine("Start time {0}", DateTime.Now);
            _logger.Info("Start batch");
            var sw = new Stopwatch();
            sw.Start();
            try
            {
                UpdateInschrijvingen();
            }
            catch (Exception ex)
            {
                _logger.FatalException("Something went wrong, application will exit", ex);
                Console.WriteLine("Something went wrong. Application exits. Check log for exception");
                Environment.Exit(-1);
            }
            sw.Stop();
            _logger.Info("End batch");
            var elapsedMinutes = sw.Elapsed.TotalMinutes;
            var milliseconds = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine("End time: {0} - elapsed minutes {1}", DateTime.Now, elapsedMinutes);
            _logger.Info("Total elapsed milliseconds {0} - minutes {1}", milliseconds, elapsedMinutes);
        }

        static void UpdateInschrijvingen()
        {
            var records = ReadInschrijvingRecords();

            var processing = new InschrijvingProcessing(IocConfig.Container, MaxDegreeOfParallelism);
            processing.RecordProcessed += (sender, e) =>
            {
                Debug.Print($"Progress={e.Progress} Inschrijvingsnaam={e.InschrijvingNaam}");
                var consoleMessage =
                    $"\rVoortgang {e.Progress}% Succesvol {e.SuccesProgress}% Bedrijfsnaam: {e.InschrijvingNaam}"
                        .Truncate(77);

                Console.Write(consoleMessage);
            };
            processing.ProcessRecords(records, "Batchprocess ImportAll");


        }

        private static IEnumerable<InschrijvingRecord> ReadInschrijvingRecords()
        {
            var repo = IocConfig.Container.GetInstance<IKvkInschrijvingRepository>();

            var inschrijvingenRecords =
                repo.Query()
                    .Select(i => new { KvkNummer = i.KvkNummer })
                    .Select(k => new InschrijvingRecord() { kvknummer = k.KvkNummer }).ToList().Distinct();

            return inschrijvingenRecords;
        }

        public static IEnumerable<InschrijvingRecord> ReadInschrijvingRecords(string fileName)
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

        #endregion



        #region S argument

        private static void SanityCheck()
        {
            Console.WriteLine("Starting the sanity check on CT environment ");

            var service = (DataserviceClient)IocConfig.Container.GetInstance<Dataservice>();
            service.Endpoint.EndpointBehaviors.OfType<ClientViaBehavior>().First().Uri = new Uri(
                service.Endpoint.EndpointBehaviors.OfType<ClientViaBehavior>().First().Uri.ToString()
                .Replace("webservices.", "webservices.preprod."));
            service.Endpoint.Address = new EndpointAddress(new Uri(service.Endpoint.Address.Uri.ToString().Replace("HRIP-Dataservice", "HRIP-DataservicePP"))
                    , service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);

            _service = service;

            var errors = new List<ophalenInschrijvingResponse>();
            var records = ReadInschrijvingRecords("Data\\CTcheck\\CT omgeving test kvknummers.csv");
            Parallel.ForEach(records, record =>
            {
                var inschrijving = GetProductTypeResponse(record.kvknummer);
                if (inschrijving == null || inschrijving.ophalenInschrijvingResponse1.meldingen.fout != null)
                {
                    errors.Add(inschrijving);
                }
                else
                {
                    Console.Write($"\rOk. Kvknummer {record.kvknummer}");
                }
            });
            Console.WriteLine();
            Console.WriteLine(errors.Any() ? $"Errors! Found {errors.Count()} errors..." : "Succes!");

            Console.ReadLine();
        }

        #endregion
    }
}
