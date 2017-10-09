using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QNH.Overheid.KernRegister.Beheer.ViewModel
{
    public class KvKItemDeleteResult
    {
        public bool Success { get; set; }
        public bool AlreadyDeleted { get; set; }
        public Exception Exception { get; set; }
        public string KvKNummer { get; set; }
        public string InschrijvingNaam { get; set; }
    }
}