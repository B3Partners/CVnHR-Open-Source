using QNH.Overheid.KernRegister.Business.Model.Debug;
using QNH.Overheid.KernRegister.Business.Model.Entities;
using System.Collections.Generic;

namespace QNH.Overheid.KernRegister.Business.Model
{
    public interface IKvkInschrijvingRepository : IRepository<KvkInschrijving>
    {
        KvkInschrijving GetLatestInschrijving(string kvkNummer);

        SbiCode GetSbiCode(string sbiCode, string omschrijving = null);

        Vestiging GetLatestVestiging(string vestigingNummer);

        IEnumerable<Vestiging> GetAllCurrentVestigingen();

        void ActualRemove(KvkInschrijving inschrijving);

        void ActualRemove(Vestiging vestiging);

        void ActualRemove<T>(T item);

        DatabaseDebugInfo GetDebugInfo();
    }
}