using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using NLog;

namespace QNH.Overheid.KernRegister.Business.Service
{
    public class NationaalGeoregisterGeocoderService : IPostcodeService
    {
        private readonly string _baseUrl;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<string, XDocument> Cache = new Dictionary<string, XDocument>();
        private const string NameSpace = "http://www.opengis.net/xls";

        public NationaalGeoregisterGeocoderService(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public string GetMunicipalityForPostcode(string postcode)
        {
            var xml = GetXml(postcode);
            if (xml == null) return null;
            var municipalityNode = xml
                .Descendants(XName.Get("Place", NameSpace))
                .FirstOrDefault(n => n.HasAttributes && n.Attributes("type").Any() && n.Attribute("type").Value == "Municipality");
            return municipalityNode?.Value;
        }

        public string GetCountrySubdivisionForPostcode(string postcode)
        {
            var xml = GetXml(postcode);
            if (xml == null) return null;
            var countrySubdivisionNode = xml
                .Descendants(XName.Get("Place", NameSpace))
                .FirstOrDefault(n => n.HasAttributes && n.Attributes("type").Any() && n.Attribute("type").Value == "CountrySubdivision");
            return countrySubdivisionNode == null ? null : countrySubdivisionNode.Value;
        }

        private XDocument GetXml(string postcode)
        {
            if (!Cache.ContainsKey(postcode))
                Cache.Add(postcode, DowloadXml(postcode));

            return Cache[postcode];
        }

        private XDocument DowloadXml(string postcode)
        {
            try
            {
                string result;
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    result = client.DownloadString(string.Format(_baseUrl, postcode));
                }

                XDocument xml = null;

                if (!string.IsNullOrEmpty(result))
                    xml = XDocument.Parse(result);

                return xml;
            }
            catch (Exception ex)
            {
                Log.DebugException("Error in PostcodeService! ", ex);
                return null;
            }
        }
    }
}