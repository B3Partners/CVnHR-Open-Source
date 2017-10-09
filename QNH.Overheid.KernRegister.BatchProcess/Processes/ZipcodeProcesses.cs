using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using CsvHelper;
using QNH.Overheid.KernRegister.Business.Service;

namespace QNH.Overheid.KernRegister.BatchProcess.Processes
{
    public class ZipcodeProcesses
    {
        #region T argument

        public static void TestPostcodeService()
        {
            var service = IocConfig.Container.GetInstance<IPostcodeService>();
            var gemeente = service.GetMunicipalityForPostcode("2013ER");
            Console.WriteLine("Gemeente for 2013ER is: {0}", gemeente);
            var provincie = service.GetCountrySubdivisionForPostcode("2013ER");
            Console.WriteLine("Provincie for 2013ER is: {0}", provincie);

            var gemeente2 = service.GetMunicipalityForPostcode("8713JL");
            Console.WriteLine("Gemeente for 8713JL is: {0}", gemeente2);
            var provincie2 = service.GetCountrySubdivisionForPostcode("8713JL");
            Console.WriteLine("Provincie for 8713JL is: {0}", provincie2);

            Console.ReadLine();
        }

        #endregion

        private const string KvKsearchUrl = "http://zoeken.kvk.nl/JsonSearch.ashx?q={0}&start={1}";

        public static IEnumerable<string> GetKvkIdsForZipcode(int maxDegreeOfParallelism, bool checkZipCodeInPostcodeField = false, params string[] zipCodes)
        {
            var records = new List<ZipCodeRecord>();
            if (!(zipCodes?.Any() ?? false))
            {
                foreach (var csvFile in Directory.GetFiles("Zipcode", "*.csv"))
                {
                    var file = new FileInfo(csvFile);
                    if (!file.Exists)
                        continue;

                    if (file.Extension.ToLower() != ".csv")
                        continue;
                    records.AddRange(ReadZipcodeRecords(file.FullName));
                }
                zipCodes = records.Select(z => z.postcode).ToArray();
            }

            var locker = new Object();
            var kvkIds = new List<string>();

            var totalHandled = 0;
            var total = records.Count;

            // Parallel foreach all currentInschrijvingen and catch the result
            Parallel.ForEach(zipCodes, new ParallelOptions() { MaxDegreeOfParallelism = maxDegreeOfParallelism },
                (postcode) =>
                {
                    // remove spaces
                    postcode = postcode.Replace(" ", string.Empty);

                    // get the kvkIds for the zipcode provided
                    var ids = new List<string>();

                    // Start all the downloading...
                    using (var webClient = new WebClient())
                    {
                        var start = 0;
                        var url = string.Format(KvKsearchUrl, postcode, start);
                        var data = Json.Decode(webClient.DownloadString(url));
                        ids.AddRange(GetKvkIdsFromEntries(data.entries, checkZipCodeInPostcodeField ? postcode : null));

                        var totalresults = Convert.ToInt32(data.pageinfo.resultscount);
                        var resultsperpage = Convert.ToInt32(data.pageinfo.resultsperpage);

                        // go on until all is collected
                        while (ids.Count < totalresults && start <= totalresults)
                        {
                            start += resultsperpage;

                            url = string.Format(KvKsearchUrl, postcode, start);
                            data = Json.Decode(webClient.DownloadString(url));
                            ids.AddRange(GetKvkIdsFromEntries(data.entries, checkZipCodeInPostcodeField ? postcode : null));
                        }
                    }

                    // finally add the kvkIds for the current zipcode to the 'global' collection
                    lock (locker)
                    {
                        kvkIds.AddRange(ids);
                    }
                    ShowPercentProgress("Processing...", totalHandled++, total);
                });
            return kvkIds;
        }

        private static IEnumerable<string> GetKvkIdsFromEntries(dynamic entries, string checkSearchInPostcode = null)
        {
            foreach (var entry in entries)
                if(checkSearchInPostcode != null && (entry.postcode as string)?.Contains(checkSearchInPostcode) == true)
                    yield return entry.dossiernummer;
        }

        private static IEnumerable<ZipCodeRecord> ReadZipcodeRecords(string fileName)
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

        public static void ShowPercentProgress(string message, int currElementIndex, int totalElementCount)
        {
            if (totalElementCount <= 1)
                return;
            if (currElementIndex < 0 || currElementIndex >= totalElementCount)
            {
                throw new InvalidOperationException("currElement out of range");
            }
            var percent = (100 * ((float)currElementIndex + 1)) / (float)totalElementCount;
            Console.Write("\r{0}{1}% complete...", message, percent);
            if (currElementIndex == totalElementCount - 1)
            {
                Console.WriteLine(Environment.NewLine);
            }
        }
    }
}
