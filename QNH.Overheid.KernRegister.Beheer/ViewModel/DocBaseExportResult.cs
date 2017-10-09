using QNH.Overheid.KernRegister.Beheer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QNH.Overheid.KernRegister.Business.Crm;

namespace QNH.Overheid.KernRegister.Beheer.ViewModel
{
    public class CrmExportResult
    {
        public virtual KvkItem KvkItem { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Succes { get; set; }
        public virtual bool NoItemsFoundInsertInstead { get; set; }
        public virtual FinancialProcesType? FinancialProcesType { get; set; } = null;
        public virtual IEnumerable<string> Errors { get; set; }

        public CrmExportResult(bool succes, string message, bool noItemsFoundInsertInstead = false, IEnumerable<string> errors = null)
        {
            Succes = succes;
            Message = message;
            Errors = errors;
            NoItemsFoundInsertInstead = noItemsFoundInsertInstead;
        } 
    }
}