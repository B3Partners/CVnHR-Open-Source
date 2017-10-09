using System.Collections.Generic;

namespace QNH.Overheid.KernRegister.Business.Crm
{
    public class ExportResult : IExportResult
    {
        public virtual string Message { get; set; }
        public virtual bool Succes { get; set; }
        public virtual bool NoItemsFoundInsertInstead { get; set; }
        public virtual IEnumerable<string> Errors { get; set; }

        public ExportResult(bool succes, string message, bool noItemsFoundInsertInstead = false, IEnumerable<string> errors = null)
        {
            Succes = succes;
            Message = message;
            Errors = errors;
            NoItemsFoundInsertInstead = noItemsFoundInsertInstead;
        }  
    }
}
