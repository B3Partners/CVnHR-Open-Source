using System.Collections.Generic;
using QNH.Overheid.KernRegister.Beheer.Models;
using QNH.Overheid.KernRegister.Business.Model.Entities;

namespace QNH.Overheid.KernRegister.Beheer.ViewModels
{
    public class KvkItem
    {
        public string KvkNummer { get; set; }
        public string Naam { get; set; }

        public int AantalVestigingen
        {
            get {
                return Vestigingen == null ? 0 : Vestigingen.Count;
            }
        }

        public KvkInschrijving Inschrijving { get; set; }
        public List<Vestiging> Vestigingen { get; set; }
        public string PeilMoment { get; set; }
        public string NaamEigenaar { get; set; }
        public string AantalMedewerkers { get;set;}
        public string PostCode { get; set; }
        public string Adres { get; set; }
        public string PostPostCode { get; set; }
        public string PostAdres { get; set; }
        public string BagId { get; set; }
        public string SBICode { get; set; }
    }
}