using System.Collections.Generic;
using NLog;
using QNH.Overheid.KernRegister.Business.Model.Entities;

namespace QNH.Overheid.KernRegister.Business.Crm
{
    public interface IFinancialExportService
    {
        IExportResult Update(KvkInschrijving kvkInschrijving, FinancialProcesType type);
        IExportResult Insert(KvkInschrijving kvkInschrijving, FinancialProcesType type);
        IExportResult UpdateVestiging(Vestiging vestiging, FinancialProcesType type);
        IExportResult InsertVestiging(Vestiging vestiging, FinancialProcesType type);

        void UpdateAllExistingVestigingen(IEnumerable<Vestiging> vestigingen, Logger functionalLogger, int maxDegreeOfParallelism, FinancialProcesType type);
    }
}