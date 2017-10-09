using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QNH.Overheid.KernRegister.Business.Model.EntityComparers;

namespace QNH.Overheid.KernRegister.Business.Model.Entities
{
    public class SbiCode : IFunctionalEquals<SbiCode>
    {
        [Key]
        public virtual string Code { get; set; }
        public virtual string Omschrijving { get; set; }

        private ICollection<SbiActiviteit> _sbiActiviteiten;
        public virtual ICollection<SbiActiviteit> SbiActiviteiten
        {
            get { return _sbiActiviteiten ?? (_sbiActiviteiten = new List<SbiActiviteit>()); }
            set { _sbiActiviteiten = value; }
        }

        private ICollection<VestigingSbiActiviteit> _vestigingSbiActiviteiten;
        public virtual ICollection<VestigingSbiActiviteit> VestigingSbiActiviteiten
        {
            get { return _vestigingSbiActiviteiten ?? (_vestigingSbiActiviteiten = new List<VestigingSbiActiviteit>()); }
            set { _vestigingSbiActiviteiten = value; }
        }

        public virtual bool FunctionalEquals(SbiCode other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return
                string.Equals(Code, other.Code)
                && string.Equals(Omschrijving, other.Omschrijving);
        }
    }
}