using CsvHelper;
using NLog;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace QNH.Overheid.KernRegister.Business.Utility
{
    public class CsvUtils
    {
        public static IEnumerable<ZipCodeRecord> ReadZipcodeRecords(string fileName)
        {
            // First read complete CSV to see how many KVKnummers we need to process
            IEnumerable<ZipCodeRecord> inschrijvingCsvRecords;
            using (TextReader reader = File.OpenText(fileName))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.BadDataFound = null;
                inschrijvingCsvRecords = csv.GetRecords<ZipCodeRecord>().ToArray();
            }

            return inschrijvingCsvRecords;
        }

        public static IEnumerable<InschrijvingRecord> ReadInschrijvingRecords(string fileName, Logger logger = null)
        {
            var configuratedHeaders = (ConfigurationManager.AppSettings["Csv-possibleheaders"] ?? "kvknummer,dossiernr,inschrijvingnummer")
                    .Split(',')
                    .Select(h => h.ToLowerInvariant());
            // First read complete CSV to see how many KVKnummers we need to process
            IEnumerable<InschrijvingRecord> inschrijvingCsvRecords;
            bool useLogger = logger != null ? true : false;
            using (TextReader reader = File.OpenText(fileName))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.ReadingExceptionOccurred = (x) => {
                    if (useLogger)
                    {
                        logger.Info($"Error in CSV: {x.GetType()} - {x.Message} for record: {x.ReadingContext.RawRecord}");
                    }
                    return false; // do not throw!
                };

                // Setup the matching headers, allow any of the configurated headers to match
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => {
                    if (useLogger)
                    {
                        logger.Debug($"Start PrepareHeaderForMatch => header: {header} - index: {index} - configuratedHeaders: {string.Join(", ", configuratedHeaders)}, - delimeter: {csv.Configuration.Delimiter}");
                    }
                    var headerLower = header.ToLowerInvariant();
                    return configuratedHeaders.Any(h => h == headerLower) ? nameof(InschrijvingRecord.kvknummer) : header;
                };
                // Log validate errors for header
                csv.Configuration.HeaderValidated = (bool success, string[] items, int index, ReadingContext ctx) => {
                    if (!success && useLogger) {
                        logger.Info($"Could not find header(s). items: {string.Join(",", items)}, index: {index}, for record: {ctx.RawRecord}");
                    }
                };

                // Set BadDataFound to log (and ignore?) bad data.
                csv.Configuration.BadDataFound = (readingContext) => {
                    if (useLogger)
                    {
                        logger.Info($"Bad data found! Error on record: {readingContext.RawRecord}");
                    }
                };

                // Set the delimeter so culture is not an issue 
                // TODO: make configurable
                csv.Configuration.Delimiter = ",";
                inschrijvingCsvRecords = csv.GetRecords<InschrijvingRecord>().ToArray();
            }

            return inschrijvingCsvRecords;
        }
    }
}
