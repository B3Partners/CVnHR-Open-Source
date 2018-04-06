using NLog;
using QNH.Overheid.KernRegister.Business.Business;
using QNH.Overheid.KernRegister.Business.Service;
using QNH.Overheid.KernRegister.Business.Service.BRMO;
using QNH.Overheid.KernRegister.Business.SignaalService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace QNH.Overheid.KernRegister.Beheer.Controllers.Api
{
    public class SignaalController : ApiController
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        // GET: api/Signaal
        public IEnumerable<string> Get() => new [] { "KvkSignaalApi", "QNH", "V1", "Implemented actions:" }
                    .Concat(GetType().GetMethods()
                                     .Where(method => method.IsPublic && method.GetCustomAttributes(false)
                                                                                .Any(a=>a is HttpPostAttribute 
                                                                                        || a is HttpGetAttribute))
                                     .Select(method=> (Request.RequestUri.ToString() + "/" + method.Name).Replace("//", "/")));


        // POST: api/Signaal
        [HttpPost, ActionName("NieuweInschrijving")]
        public async Task NieuweInschrijving(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentException("request");
            }

            var kvkNummer = ParseKvKnummerFromHttpRequestMessage(request);
            _log.Info($"Received new NieuweInschrijving with KvkNummer: {kvkNummer}");

            UpdateOrInsertInschrijving(kvkNummer);
        }

        // POST: api/Signaal
        [HttpPost, ActionName("NieuweInschrijvingBrmo")]
        public async Task NieuweInschrijvingBrmo(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentException("request");
            }

            var kvkNummer = ParseKvKnummerFromHttpRequestMessage(request);
            _log.Info($"Received new NieuweInschrijvingBrmo with KvkNummer: {kvkNummer}");

            UpdateOrInsertInschrijvingBrmo(kvkNummer);
        }

        [HttpPost, ActionName("Signaal")]
        public async Task Signaal(HttpRequestMessage request)
        {
            var kvkNummer = ParseKvKnummerFromHttpRequestMessage(request);
            _log.Info($"Received new Signaal with KvkNummer: {kvkNummer}");
            UpdateOrInsertInschrijving(kvkNummer);
        }

        [HttpPost, ActionName("SignaalBrmo")]
        public async Task SignaalBrmo(HttpRequestMessage request)
        {
            var kvkNummer = ParseKvKnummerFromHttpRequestMessage(request);
            _log.Info($"Received new SignaalBrmo with KvkNummer: {kvkNummer}");
            UpdateOrInsertInschrijvingBrmo(kvkNummer);
        }

        [HttpPost, ActionName("Bericht")]
        public async Task Bericht(HttpRequestMessage request)
        {
            var kvkNummer = ParseKvKnummerFromHttpRequestMessage(request);
            _log.Info($"Received new Bericht with KvkNummer: {kvkNummer}");
            UpdateOrInsertInschrijving(kvkNummer);
        }

        [HttpPost, ActionName("BerichtBrmo")]
        public async Task BerichtBrmo(HttpRequestMessage request)
        {
            var kvkNummer = ParseKvKnummerFromHttpRequestMessage(request);
            _log.Info($"Received new BerichtBrmo with KvkNummer: {kvkNummer}");
            UpdateOrInsertInschrijvingBrmo(kvkNummer);
        }

        private string ParseKvKnummerFromHttpRequestMessage(HttpRequestMessage request)
        {
            var xml = request.Content.ReadAsStringAsync().Result;
            _log.Trace($"Request xml: {xml}");

            if (string.IsNullOrWhiteSpace(xml))
            {
                _log.Error($"Cannot parse kvknummer from empty string! Could not read xml from request. RequestUri: {request.RequestUri}");
                return null;
            }

            // TODO: rewrite using Regexes.
            var closingTag = "</kvknummer";
            var reverseXml = new string(xml.Reverse().ToArray());
            var indexOfClosingTag = reverseXml.IndexOf(new string(closingTag.Reverse().ToArray()), StringComparison.InvariantCultureIgnoreCase) + closingTag.Length;
            if (indexOfClosingTag > 0)
            {
                var indexOfStartTag = reverseXml.IndexOf(">", indexOfClosingTag, StringComparison.CurrentCultureIgnoreCase);
                var kvknummer = new string(reverseXml.Substring(indexOfClosingTag, indexOfStartTag - indexOfClosingTag).Reverse().ToArray());
                _log.Trace($"Parsed kvknummer {kvknummer} from request: {kvknummer}");
                return kvknummer;
            }

            _log.Error($"Xml received does not contain kvknummer! xml: {xml}");
            return null;
        }

        private void UpdateOrInsertInschrijving(string kvkNummer)
        {
            try
            {
                var service = IocConfig.Container.GetInstance<IKvkSearchService>();
                var kvkInschrijving = service.SearchInschrijvingByKvkNummer(kvkNummer);
                var storageService = IocConfig.Container.GetInstance<IInschrijvingSyncService>();
                var status = storageService.AddNewInschrijvingIfDataChanged(kvkInschrijving);
                _log.Trace($"Inschrijving status: {status}");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw ex;
            }
        }

        private void UpdateOrInsertInschrijvingBrmo(string kvkNummer)
        {
            try
            {
                var hrDataserviceVersionNumberBrmo = ConfigurationManager.AppSettings["HR-DataserviceVersionNumberBrmo"];
                var service = hrDataserviceVersionNumberBrmo == "2.5"
                    ? IocConfig.Container.GetInstance<IKvkSearchServiceV25>()
                    : IocConfig.Container.GetInstance<IKvkSearchService>();
                var kvkInschrijving = service.SearchInschrijvingByKvkNummer(kvkNummer);

                // retry with bypassing cache
                var xDoc = RawXmlCache.Get(kvkNummer,
                    () => { kvkInschrijving = service.SearchInschrijvingByKvkNummer(kvkNummer, true); });

                var brmoSyncService = IocConfig.Container.GetInstance<IBrmoSyncService>();
                var status = brmoSyncService.UploadXDocumentToBrmo(xDoc);
                brmoSyncService.Transform(kvkNummer);
                _log.Trace($"Inschrijving status: {status}");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw ex;
            }
        }
    }
}
