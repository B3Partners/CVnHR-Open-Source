using QNH.Overheid.KernRegister.Business.KvK;
using QNH.Overheid.KernRegister.Business.KvK.v30;
using QNH.Overheid.KernRegister.Business.Model.Entities;

namespace QNH.Overheid.KernRegister.Business.Service
{
    public interface IKvkSearchService
    {
        KvkInschrijving SearchInschrijvingByKvkNummer(string kvkNummer, string requesterName, bool bypassCache = false);

        Vestiging SearchVestigingByVestigingsNummer(string vestigingsNummer, string kvkNummer = null);

        dynamic GetInschrijvingResponseTypeByKvkNummer(string kvkNummer, bool bypassCache = false);

        InschrijvingResponseType DoeOpvragingBijKvk(string kvkNummer, string requesterName);
    }
}