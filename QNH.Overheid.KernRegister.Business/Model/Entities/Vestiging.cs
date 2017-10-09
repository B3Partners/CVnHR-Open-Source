using QNH.Overheid.KernRegister.Business.Model.nHibernate;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QNH.Overheid.KernRegister.Business.Model.EntityComparers;

namespace QNH.Overheid.KernRegister.Business.Model.Entities
{
    /// <summary>
    /// Vestiging entiteit
    /// </summary>
    public class Vestiging : IFunctionalEquals<Vestiging>
    {
        public const string RECHTSPERSOONPREFIX = "[Rechtspersoon]: ";

        [Indexable]
        public virtual int Id { get; set; }
        [Indexable]
        public virtual string Vestigingsnummer { get; set; }

        [Indexable]
        public virtual string Naam { get; set; }
        public virtual string Adres { get; set; }
        public virtual string Straat { get; set; }
        public virtual string Huisnummer { get; set; }
        public virtual string Huisnummertoevoeging { get; set; }
        public virtual string PostcodeCijfers { get; set; }
        public virtual string PostcodeLetters { get; set; }
        public virtual string Woonplaats { get; set; }
        public virtual string Telefoon { get; set; }
        public virtual string Fax { get; set; }
        public virtual string Email { get; set; }
        public virtual string Gemeente { get; set; }

        [Indexable]
        public virtual string BagId { get; set; }
        [Indexable]
        public virtual string RSIN { get; set; }
        public virtual string EORI { get;set; }

        public virtual DateTime IngevoegdOp { get; set; }
        public virtual DateTime GeldigTot { get; set; }
        public virtual bool IsHoofdvestiging { get; set; }
        public virtual string Postbusnummer { get; set; }
        public virtual string PostAdres { get; set; }
        public virtual string PostStraat { get; set; }
        public virtual string PostHuisnummer { get; set; }
        public virtual string PostHuisnummerToevoeging { get; set; }
        public virtual string PostPostcodeCijfers { get; set; }
        public virtual string PostPostcodeLetters { get; set; }
        public virtual string PostWoonplaats { get; set; }
        public virtual string PostGemeente { get; set; }


        public virtual string RegistratieDatumAanvang { get; set; }
        public virtual string RegistratieDatumEinde { get; set; }

        private ICollection<VestigingSbiActiviteit> _sbiActiviteiten;
        public virtual ICollection<VestigingSbiActiviteit> SbiActiviteiten
        {
            get { return _sbiActiviteiten ?? (_sbiActiviteiten = new List<VestigingSbiActiviteit>()); }
            set { _sbiActiviteiten = value; }
        }

        public virtual string FulltimeWerkzamePersonen { get; set; }
        public virtual string ParttimeWerkzamePersonen { get; set; }
        public virtual string TotaalWerkzamePersonen { get; set; }

        [Required]
        public virtual KvkInschrijving KvkInschrijving { get; set; }

        public virtual bool FunctionalEquals(Vestiging other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Vestigingsnummer, other.Vestigingsnummer)
                && string.Equals(Naam, other.Naam)
                && string.Equals(Adres, other.Adres)
                && string.Equals(Straat, other.Straat)
                && string.Equals(Huisnummer, other.Huisnummer)
                && string.Equals(Huisnummertoevoeging ?? "", other.Huisnummertoevoeging ?? "")
                && string.Equals(PostcodeCijfers, other.PostcodeCijfers)
                && string.Equals(PostcodeLetters, other.PostcodeLetters)
                && string.Equals(Woonplaats, other.Woonplaats)
                && string.Equals(Telefoon, other.Telefoon)
                && string.Equals(Fax, other.Fax)
                && string.Equals(Email, other.Email)
                && string.Equals(Gemeente, other.Gemeente)
                && IsHoofdvestiging.Equals(other.IsHoofdvestiging)
                && string.Equals(Postbusnummer, other.Postbusnummer)
                && string.Equals(PostAdres, other.PostAdres)
                && string.Equals(PostStraat, other.PostStraat)
                && string.Equals(PostHuisnummer, other.PostHuisnummer)
                && string.Equals(PostHuisnummerToevoeging ?? "", other.PostHuisnummerToevoeging ?? "")
                && string.Equals(PostPostcodeCijfers, other.PostPostcodeCijfers)
                && string.Equals(PostPostcodeLetters, other.PostPostcodeLetters)
                && string.Equals(PostWoonplaats, other.PostWoonplaats)
                && string.Equals(PostGemeente, other.PostGemeente)
                && string.Equals(EORI, other.EORI)
                && string.Equals(RSIN, other.RSIN)
                && string.Equals(BagId, other.BagId)
                && string.Equals(FulltimeWerkzamePersonen ?? "", other.FulltimeWerkzamePersonen ?? "")
                && string.Equals(ParttimeWerkzamePersonen ?? "", other.ParttimeWerkzamePersonen ?? "")
                && string.Equals(TotaalWerkzamePersonen ?? "", other.TotaalWerkzamePersonen ?? "")
                && string.Equals(RegistratieDatumAanvang, other.RegistratieDatumAanvang)
                && string.Equals(RegistratieDatumEinde, other.RegistratieDatumEinde)

                && SbiActiviteiten.FunctionalEquals(other.SbiActiviteiten);
        }
    }
}