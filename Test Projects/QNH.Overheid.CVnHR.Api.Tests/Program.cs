using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Threading;

namespace QNH.Overheid.CVnHR.Api.Tests
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var kvknummer = ParseKvKnummerFromHttpRequestMessage(Messages.NewNieuweInschrijving);
            //Console.WriteLine(kvknummer);
            //var kvknummer2 = ParseKvKnummerFromHttpRequestMessage(Messages.OldNieuweInschrijving);
            //Console.WriteLine(kvknummer2);


            var apiUrl = "http://kvk.local/api/signaal/signaal";

            Enumerable.Range(0, 20).ToList().ForEach(i =>
            {
                new Thread(() => DoRequest(apiUrl)).Start();
            });

            Console.ReadLine();
        }

        private static void DoRequest(string apiUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(apiUrl);
            //var bytes = System.Text.Encoding.ASCII.GetBytes(Messages.OldNieuweInschrijving);
            var bytes = System.Text.Encoding.ASCII.GetBytes(Messages.NewNieuweInschrijving);
            request.ContentType = "text/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            var requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            var responseStream = response.GetResponseStream();
            string responseStr = new StreamReader(responseStream).ReadToEnd();
            Console.WriteLine($"Response statuscode: {response.StatusCode}, Response: {responseStr}");
        }

        private static string ParseKvKnummerFromHttpRequestMessage(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return null;
            }
            var closingTag = "</kvknummer";
            var reverseXml = new string(xml.Reverse().ToArray());
            var indexOfClosingTag = reverseXml.IndexOf(new string(closingTag.Reverse().ToArray()), StringComparison.InvariantCultureIgnoreCase) + closingTag.Length;
            if (indexOfClosingTag > 0)
            {
                var indexOfStartTag = reverseXml.IndexOf(">", indexOfClosingTag, StringComparison.CurrentCultureIgnoreCase);
                return new string(reverseXml.Substring(indexOfClosingTag, indexOfStartTag - indexOfClosingTag).Reverse().ToArray());
            }
            else
            {
                return null;
            }
        }
    }
}
