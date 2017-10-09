using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Engine;
using QNH.Overheid.KernRegister.Business.Model.EntityComparers;

namespace QNH.Overheid.KernRegister.Business.Model.Entities
{
    public class HandelsNaam : IFunctionalEquals<HandelsNaam>
    {
        public virtual int Id { get; set; }
        public virtual string Handelsnaam { get; set; }

        [Required]
        public virtual KvkInschrijving KvkInschrijving { get; set; }

        public virtual bool FunctionalEquals(HandelsNaam other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return string.Equals(Handelsnaam, other.Handelsnaam);
        }
    }
}
