using CsvHelper;
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
                inschrijvingCsvRecords = csv.GetRecords<ZipCodeRecord>().ToArray();
            }

            return inschrijvingCsvRecords;
        }

        public static IEnumerable<InschrijvingRecord> ReadInschrijvingRecords(string fileName)
        {
            // First read complete CSV to see how many KVKnummers we need to process
            IEnumerable<InschrijvingRecord> inschrijvingCsvRecords;
            using (TextReader reader = File.OpenText(fileName))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.HeaderValidated = null;
                // Setup the matching headers, allow any of the configurated headers to match
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => {
                    var headerLower = header.ToLower();
                    var configuratedHeaders = (ConfigurationManager.AppSettings["Csv-possibleheaders"] ?? "kvknummer,dossiernr,inschrijvingnummer")
                            .Split(',')
                            .Select(h => h.ToLowerInvariant());
                    return configuratedHeaders.Any(h => h == headerLower) ? nameof(InschrijvingRecord.kvknummer) : header;
                };
                inschrijvingCsvRecords = csv.GetRecords<InschrijvingRecord>().ToArray();
            }

            return inschrijvingCsvRecords;
        }
    }
}
