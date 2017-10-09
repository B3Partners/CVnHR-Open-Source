using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QNH.Overheid.KernRegister.Business.Model.EntityComparers;

namespace QNH.Overheid.KernRegister.Business.Model.Entities
{
    public class VestigingSbiActiviteit : IFunctionalEquals<VestigingSbiActiviteit>
    {
        public virtual int Id { get; set; }

        [Required]
        public virtual SbiCode SbiCode { get; set; }
        
        [Required]
        public virtual Vestiging Vestiging { get; set; }
        
        public virtual bool IsHoofdSbiActiviteit { get; set; }

        public virtual bool FunctionalEquals(VestigingSbiActiviteit other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var isVestigingEqual = false;
            if (Vestiging == null && other.Vestiging == null)
                isVestigingEqual = true;
            else if (Vestiging == null || other.Vestiging == null)
                isVestigingEqual = false;
            else
                isVestigingEqual = Equals(Vestiging.Vestigingsnummer, other.Vestiging.Vestigingsnummer);

            var equals = bool.Equals(IsHoofdSbiActiviteit, other.IsHoofdSbiActiviteit)
                && SbiCode?.FunctionalEquals(other.SbiCode) == true
                && isVestigingEqual;

            return equals;
        }
    }
}