using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using QNH.Overheid.KernRegister.Business.Model.EntityComparers;

namespace QNH.Overheid.KernRegister.Business.Model.Entities
{
    public class DeponeringsStuk : IFunctionalEquals<DeponeringsStuk>
    {
        public virtual int Id { get; set; }
        public virtual string DepotId { get; set; }
        public virtual DateTime DatumDeponering { get; set; }
        public virtual string Type { get; set; }

        public virtual string Status { get; set; }
        public virtual string GaatOver { get; set; }

        [Required]
        public virtual KvkInschrijving KvkInschrijving { get; set; }

        public virtual bool FunctionalEquals(DeponeringsStuk other)
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
                string.Equals(DepotId, other.DepotId)
                && DateTime.Equals(DatumDeponering, other.DatumDeponering)
                && string.Equals(Type, other.Type)
                && string.Equals(Status, other.Status)
                && string.Equals(GaatOver, other.GaatOver);
        }
    }
}