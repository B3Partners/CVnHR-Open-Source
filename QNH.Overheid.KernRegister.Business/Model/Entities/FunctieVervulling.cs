using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using QNH.Overheid.KernRegister.Business.Model.EntityComparers;

namespace QNH.Overheid.KernRegister.Business.Model.Entities
{
    public class FunctieVervulling : IFunctionalEquals<FunctieVervulling> // : IEquatable<FunctieVervulling>, IEqualityComparer<FunctieVervulling>
    {
        public virtual int Id { get; set; }

        public virtual string Functie { get; set; }
        public virtual string FunctieTitel { get; set; }
        public virtual string VolledigeNaam { get; set; }
        
        public virtual string Schorsing { get; set; }
        public virtual string LangstZittende { get; set; }

        public virtual string Bevoegdheid { get; set; }
        public virtual string HandelingsBekwaam { get; set; }

        [Required]
        public virtual KvkInschrijving KvkInschrijving { get; set; }

        public virtual bool FunctionalEquals(FunctieVervulling other)
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
                string.Equals(Functie, other.Functie)
                && string.Equals(FunctieTitel, other.FunctieTitel)
                && string.Equals(VolledigeNaam, other.VolledigeNaam)
                && string.Equals(Schorsing, other.Schorsing)
                && string.Equals(LangstZittende, other.LangstZittende)
                && string.Equals(Bevoegdheid, other.Bevoegdheid)
                && string.Equals(HandelingsBekwaam, other.HandelingsBekwaam);
        }

        //public virtual bool Equals(FunctieVervulling other)
        //{
        //    if (ReferenceEquals(null, other))
        //    {
        //        return false;
        //    }
        //    if (ReferenceEquals(this, other))
        //    {
        //        return true;
        //    }

        //    return
        //        string.Equals(Functie, other.Functie)
        //        && string.Equals(FunctieTitel, other.FunctieTitel)
        //        && string.Equals(VolledigeNaam, other.VolledigeNaam)
        //        && string.Equals(Schorsing, other.Schorsing)
        //        && string.Equals(LangstZittende, other.LangstZittende)
        //        && string.Equals(Bevoegdheid, other.Bevoegdheid)
        //        && string.Equals(HandelingsBekwaam, other.HandelingsBekwaam);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj))
        //    {
        //        return false;
        //    }
        //    if (ReferenceEquals(this, obj))
        //    {
        //        return true;
        //    }
        //    //TODO: take care of nHibernate proxy types!
        //    //if (obj.GetType() != this.GetType())
        //    //{
        //    //    return false;
        //    //}
        //    return Equals((FunctieVervulling)obj);
        //}

        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        var hashCode = 389;
        //        hashCode = (hashCode * 397) + Functie?.GetHashCode() ?? 0;
        //        hashCode = (hashCode * 397) + (FunctieTitel?.GetHashCode() ?? 0);
        //        hashCode = (hashCode * 397) + (VolledigeNaam?.GetHashCode() ?? 0);
        //        hashCode = (hashCode * 397) + (Schorsing?.GetHashCode() ?? 0);
        //        hashCode = (hashCode * 397) + (LangstZittende?.GetHashCode() ?? 0);
        //        hashCode = (hashCode * 397) + (Bevoegdheid?.GetHashCode() ?? 0);
        //        hashCode = (hashCode * 397) + (HandelingsBekwaam?.GetHashCode() ?? 0);
        //        return hashCode;
        //    }
        //}

        //public virtual bool Equals(FunctieVervulling x, FunctieVervulling y) => x.Equals(y);

        //public virtual int GetHashCode(FunctieVervulling obj) => obj.GetHashCode();
    }
}
