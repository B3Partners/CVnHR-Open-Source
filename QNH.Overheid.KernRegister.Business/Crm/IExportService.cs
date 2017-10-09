using System.Collections.Generic;
using NLog;
using QNH.Overheid.KernRegister.Business.Model.Entities;

namespace QNH.Overheid.KernRegister.Business.Crm
{
    public interface IExportService
    {
        IExportResult UpdateExternalRecord(KvkInschrijving kvkInschrijving);

        IExportResult InsertExternalRecord(KvkInschrijving kvkInschrijving);

        IExportResult UpdateExternalVestiging(Vestiging vestiging);

        IExportResult InsertExternalVestiging(Vestiging vestiging);

        void UpdateAllExistingExternalVestigingen(IEnumerable<Vestiging> vestigingen, Logger functionalLogger, int maxDegreeOfParallelism);
    }
}