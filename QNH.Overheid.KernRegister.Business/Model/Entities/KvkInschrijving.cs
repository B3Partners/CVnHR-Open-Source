using QNH.Overheid.KernRegister.Business.Model.nHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using QNH.Overheid.KernRegister.Business.Model.EntityComparers;

namespace QNH.Overheid.KernRegister.Business.Model.Entities
{
    public class KvkInschrijving : IFunctionalEquals<KvkInschrijving>
    {
        public const string Separator = " | ";

        public virtual int Id { get; set; }

        [Indexable]
        public virtual string Naam { get; set; }

        public virtual string InschrijvingNaam => Naam;

        [Indexable]
        public virtual string KvkNummer { get; set; }
        public virtual string Peilmoment { get; set; }
        public virtual DateTime IngevoegdOp { get; set; }
        public virtual DateTime GeldigTot { get; set; }

        public virtual DateTime OpgevraagdOp { get; set; }
        public virtual string RegistratieDatumAanvang { get; set; }
        public virtual string RegistratieDatumEinde { get; set; }
        public virtual string DatumOprichting { get; set; }
        public virtual string DatumUitschrijving { get; set; }

        public virtual string PersoonRechtsvorm { get; set; }
        public virtual string UitgebreideRechtsvorm { get; set; }
        public virtual string BijzondereRechtsToestand { get; set; }
        public virtual string RedenInsolventie { get; set; }
        public virtual string BeperkingInRechtshandeling { get; set; }
        public virtual string EigenaarHeeftGedeponeerd { get; set; }

        public virtual string VolledigeNaamEigenaar { get; set; }

        public virtual string FulltimeWerkzamePersonen { get; set; }
        public virtual string ParttimeWerkzamePersonen { get; set; }
        public virtual string TotaalWerkzamePersonen { get; set; }
        public virtual string GeplaatstKapitaal { get; set; }
        public virtual string GestortKapitaal { get; set; }

        private ICollection<FunctieVervulling> _functieVervullingen;
        public virtual ICollection<FunctieVervulling> FunctieVervullingen
        {
            get { return _functieVervullingen ?? (_functieVervullingen = new List<FunctieVervulling>()); }
            set { _functieVervullingen = value; }
        }

        private ICollection<SbiActiviteit> _sbiActiviteiten;
        public virtual ICollection<SbiActiviteit> SbiActiviteiten
        {
            get { return _sbiActiviteiten ?? (_sbiActiviteiten = new List<SbiActiviteit>()); }
            set { _sbiActiviteiten = value; }
        }

        private ICollection<DeponeringsStuk> _deponeringen;
        public virtual ICollection<DeponeringsStuk> Deponeringen
        {
            get { return _deponeringen ?? (_deponeringen = new List<DeponeringsStuk>()); }
            set { _deponeringen = value; }
        }

        private ICollection<Vestiging> _vestigingen;
        public virtual ICollection<Vestiging> Vestigingen
        {
            get { return _vestigingen ?? (_vestigingen = new List<Vestiging>()); }
            set { _vestigingen = value; }
        }

        #region v2.0 Additions

        public virtual string RechterlijkeUitspraak { get; set; }

        private ICollection<HandelsNaam> _handelsNamen;
        public virtual ICollection<HandelsNaam> HandelsNamen {
            get { return _handelsNamen ?? (_handelsNamen = new List<HandelsNaam>()); }
            set { _handelsNamen = value; }
        }
        public virtual string BerichtenBoxNaam { get; set; }

        #endregion

        #region v3.0 Additions

        public virtual string NonMailing { get; set; }

        #endregion

        public virtual bool FunctionalEquals(KvkInschrijving other)
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
                string.Equals(Naam, other.Naam)
                && string.Equals(KvkNummer, other.KvkNummer)

                && string.Equals(RegistratieDatumAanvang, other.RegistratieDatumAanvang)
                && string.Equals(RegistratieDatumEinde, other.RegistratieDatumEinde)
                && string.Equals(DatumOprichting, other.DatumOprichting)
                && string.Equals(DatumUitschrijving, other.DatumUitschrijving)
                && string.Equals(PersoonRechtsvorm, other.PersoonRechtsvorm)
                && string.Equals(UitgebreideRechtsvorm, other.UitgebreideRechtsvorm)
                && string.Equals(BijzondereRechtsToestand, other.BijzondereRechtsToestand)
                && string.Equals(RedenInsolventie, other.RedenInsolventie)
                && string.Equals(BeperkingInRechtshandeling, other.BeperkingInRechtshandeling)
                && string.Equals(EigenaarHeeftGedeponeerd, other.EigenaarHeeftGedeponeerd)
                && string.Equals(VolledigeNaamEigenaar, other.VolledigeNaamEigenaar)
                && string.Equals(FulltimeWerkzamePersonen, other.FulltimeWerkzamePersonen)
                && string.Equals(ParttimeWerkzamePersonen, other.ParttimeWerkzamePersonen)
                && string.Equals(TotaalWerkzamePersonen, other.TotaalWerkzamePersonen)
                && string.Equals(GeplaatstKapitaal, other.GeplaatstKapitaal)
                && string.Equals(GestortKapitaal, other.GestortKapitaal)
                && string.Equals(BerichtenBoxNaam, other.BerichtenBoxNaam)
                && string.Equals(RechterlijkeUitspraak, other.RechterlijkeUitspraak)
                && string.Equals(NonMailing, other.NonMailing)

                && Vestigingen.FunctionalEquals(other.Vestigingen)
                && FunctieVervullingen.FunctionalEquals(other.FunctieVervullingen)
                && SbiActiviteiten.FunctionalEquals(other.SbiActiviteiten)
                && Deponeringen.FunctionalEquals(other.Deponeringen)
                && HandelsNamen.FunctionalEquals(other.HandelsNamen);
        }
    }
}