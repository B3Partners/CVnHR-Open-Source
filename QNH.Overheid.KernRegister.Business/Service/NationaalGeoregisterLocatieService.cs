using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using NLog;

namespace QNH.Overheid.KernRegister.Business.Service
{
    public class NationaalGeoregisterLocatieService : IPostcodeService
    {
        private readonly string _baseUrl;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<string, dynamic> Cache = new Dictionary<string, dynamic>();

        public NationaalGeoregisterLocatieService(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public string GetMunicipalityForPostcode(string postcode)
        {
            var locatie = GetLocatie(postcode);
            if (locatie == null) return null;
            return locatie.gemeentenaam;
        }

        public string GetCountrySubdivisionForPostcode(string postcode)
        {
            var locatie = GetLocatie(postcode);
            if (locatie == null) return null;
            return locatie.provincienaam;
        }

        private dynamic GetLocatie(string postcode)
        {
            if (!Cache.ContainsKey(postcode))
                Cache.Add(postcode, DownloadLocatie(postcode));

            return Cache[postcode];
        }

        private dynamic DownloadLocatie(string postcode)
        {
            try
            {
                string result;
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    result = client.DownloadString(string.Format(_baseUrl, postcode));
                }

                var json = Json.Decode(result);
                if (json.response.numFound > 0)
                {
                    var maxScore = ((IEnumerable<dynamic>)json.response.docs)
                        .FirstOrDefault(doc => doc.score == json.response.maxScore);
                    return maxScore;
                }

                return null;
            }
            catch (Exception ex)
            {
                Log.Debug(ex, $"Error downloading json from NationaalGeoregisterLocatieService for postcode {postcode}");
                return null;
            }
        }
    }
}
