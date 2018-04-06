using QNH.Overheid.KernRegister.Business.KvK;
using QNH.Overheid.KernRegister.Business.Model.Entities;

namespace QNH.Overheid.KernRegister.Business.Service
{
    public interface IKvkSearchService
    {
        KvkInschrijving SearchInschrijvingByKvkNummer(string kvkNummer, bool bypassCache = false);

        Vestiging SearchVestigingByVestigingsNummer(string vestigingsNummer, string kvkNummer = null);
    }

    public interface IKvkSearchServiceV25 : IKvkSearchService
    {

    }
}