using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using QNH.Overheid.KernRegister.Business.Model.EntityComparers;

namespace QNH.Overheid.KernRegister.Business.Model.Entities
{
    public class SbiActiviteit : IFunctionalEquals<SbiActiviteit>
    {
        public virtual int Id { get; set; }

        [Required]
        public virtual SbiCode SbiCode { get; set; }

        [Required]
        public virtual KvkInschrijving KvKInschrijving { get; set; }
        
        public virtual bool IsHoofdSbiActiviteit { get; set; }

        public virtual bool FunctionalEquals(SbiActiviteit other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var isInschrijvingEqual = false;
            if (KvKInschrijving == null && other.KvKInschrijving == null)
                isInschrijvingEqual = true;
            else if (KvKInschrijving == null || other.KvKInschrijving == null)
                isInschrijvingEqual = false;
            else
                isInschrijvingEqual = Equals(KvKInschrijving.KvkNummer, other.KvKInschrijving.KvkNummer);

            var equals = bool.Equals(IsHoofdSbiActiviteit, other.IsHoofdSbiActiviteit)
                && SbiCode.FunctionalEquals(other.SbiCode)
                && isInschrijvingEqual;

            return equals;
        }
    }
}
