using QNH.Overheid.KernRegister.Business.Enums;
using QNH.Overheid.KernRegister.Business.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    public class ImportResultViewModel
    {
        public string Message { get; set; }
        public KvkInschrijving KvkInschrijving { get; set; }
        public AddInschrijvingResultStatus Status { get; set; }
    }
}