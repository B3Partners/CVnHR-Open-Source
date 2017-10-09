using System.Collections.Generic;

namespace QNH.Overheid.KernRegister.Business.Crm
{
    public interface IExportResult
    {
        bool Succes { get; set; }
        bool NoItemsFoundInsertInstead { get; set; }
        string Message { get; set; }
        IEnumerable<string> Errors { get; set; }
    }
}