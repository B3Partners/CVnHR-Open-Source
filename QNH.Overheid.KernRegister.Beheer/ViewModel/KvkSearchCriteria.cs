using System.ComponentModel;

namespace QNH.Overheid.KernRegister.Beheer.ViewModels
{
    public class KvkSearchCriteria
    {
        public KvkSearchCriteria()
        {
            All = true;
        }

        public string GlobalCriterium { get; set; }
        public string KvkNummer { get; set; }
        public string Page { get; set; }

        [DisplayName("Alles")]
        public bool All { get; set; }
        [DisplayName("KvkNummer")]
        public bool KvkNummerSearch { get; set; }
        [DisplayName("Vestigingsnummer")]
        public bool VestigingsNummer { get; set; }
        [DisplayName("Aantal medewerkers (meer dan, vul een nummer in)")]
        public bool AantalMedewerkers { get; set; }
        [DisplayName("Naam")]
        public bool Naam { get; set; }
        [DisplayName("Naam eigenaar")]
        public bool NaamEigenaar { get; set; }
        [DisplayName("Adres")]
        public bool Adres { get; set; }
        [DisplayName("Postcode")]
        public bool Postcode { get; set; }
        //[DisplayName("PostAdres")]
        //public bool PostAdres { get; set; }
        [DisplayName("BagId")]
        public bool BagId { get; set; }
        [DisplayName("SBI code")]
        public bool SBICode { get; set; }
        

        /// <summary>
        /// Gets whether or there are no searchcriteria supplied
        /// </summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(GlobalCriterium + KvkNummer);
    }
}