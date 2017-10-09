using NLog;
using QNH.Overheid.KernRegister.Business.Business;
using QNH.Overheid.KernRegister.Business.Service;
using QNH.Overheid.KernRegister.Business.Service.BRMO;
using QNH.Overheid.KernRegister.Business.SignaalService;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task NieuweInschrijving([FromBody]NieuweInschrijvingType inschrijving)
        {
            if (inschrijving == null)
            {
                throw new ArgumentException("inschrijving");
            }

            var kvkNummer = inschrijving.heeftBetrekkingOp.kvkNummer;
            _log.Trace($"Received new inschrijving with signaalId {inschrijving.signaalId} with KvkNummer: {kvkNummer}");
            _log.Trace($"Aanleiding: {inschrijving?.aanleiding?.omschrijving}");

            UpdateOrInsertInschrijving(kvkNummer);
        }

        // POST: api/Signaal
        [HttpPost, ActionName("NieuweInschrijvingBrmo")]
        public async Task NieuweInschrijvingBrmo([FromBody]NieuweInschrijvingType inschrijving)
        {
            if (inschrijving == null)
            {
                throw new ArgumentException("inschrijving");
            }

            var kvkNummer = inschrijving.heeftBetrekkingOp.kvkNummer;
            _log.Trace($"Received new inschrijving with signaalId {inschrijving.signaalId} with KvkNummer: {kvkNummer}");
            _log.Trace($"Aanleiding: {inschrijving.aanleiding.omschrijving}");

            UpdateOrInsertInschrijvingBrmo(kvkNummer);
        }

        [HttpPost, ActionName("Signaal")]
        public async Task Signaal([FromBody]SignaalType signaal)
        {
            var kvkNummer = GetKvKNummerFromSignaal(signaal);
            _log.Trace($"Received new signaal with signaalId {signaal?.signaalId} with KvkNummer: {kvkNummer}");
            UpdateOrInsertInschrijving(kvkNummer);
        }

        [HttpPost, ActionName("SignaalBrmo")]
        public async Task SignaalBrmo([FromBody] SignaalType signaal)
        {
            var kvkNummer = GetKvKNummerFromSignaal(signaal);
            _log.Trace($"Received new signaalBrmo with signaalId {signaal.signaalId} with KvkNummer: {kvkNummer}");
            UpdateOrInsertInschrijvingBrmo(kvkNummer);
        }

        [HttpPost, ActionName("Bericht")]
        public async Task Bericht([FromBody] BerichtType bericht)
        {
            var kvkNummer = GetKvkNummerFromBerichtType(bericht);
            _log.Trace($"Received new Bericht with berichtId {bericht.berichtId} with KvkNummer: {kvkNummer}");
            UpdateOrInsertInschrijving(kvkNummer);
        }

        [HttpPost, ActionName("BerichtBrmo")]
        public async Task BerichtBrmo([FromBody] BerichtType bericht)
        {
            var kvkNummer = GetKvkNummerFromBerichtType(bericht);
            _log.Trace($"Received new BerichtBrmo with berichtId {bericht.berichtId} with KvkNummer: {kvkNummer}");
            UpdateOrInsertInschrijvingBrmo(kvkNummer);
        }

        private string GetKvKNummerFromSignaal(SignaalType signaal)
        {
            string kvkNummer;
            if (signaal is NieuweInschrijvingType)
            {
                kvkNummer = ((NieuweInschrijvingType)signaal)?.heeftBetrekkingOp?.kvkNummer;
            }
            else if (signaal is InsolventiewijzigingType)
            {
                kvkNummer = ((InsolventiewijzigingType)signaal)?.heeftBetrekkingOp?.kvkNummer;
            }
            else if (signaal is VoortzettingEnOverdrachtSignaalType)
            {
                kvkNummer = ((VoortzettingEnOverdrachtSignaalType)signaal)?.heeftBetrekkingOp?.kvkNummer;
            }
            else if (signaal is RechtsvormwijzigingType)
            {
                kvkNummer = ((RechtsvormwijzigingType)signaal)?.heeftBetrekkingOp?.kvkNummer;
            }
            else
            {
                throw new NotImplementedException($"SignaalType {signaal.GetType()} not implemented...");
            }

            return kvkNummer;
        }

        private string GetKvkNummerFromBerichtType(BerichtType bericht)
        {
            string kvkNummer;
            var berichtType = bericht as UpdateBerichtType;
            if (berichtType != null)
            {
                kvkNummer = berichtType?.heeftBetrekkingOp?.kvkNummer;
            }
            else
            {
                throw new NotImplementedException($"BerichtType {bericht.GetType()} not implemented...");
            }

            return kvkNummer;
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
                var service = IocConfig.Container.GetInstance<IKvkSearchService>();
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
