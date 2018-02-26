using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.Business.KvKSearchApi.Entities
{
    public class KvkSearchApiParameters
    {
        public string Q { get; set; }
        public string KvkNumber { get; set; }
        public string BranchNumber { get; set; }
        public string Rsin { get; set; }
        public string TradeName { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string Postalcode { get; set; }
        public string City { get; set; }
        public bool IncludeFormerTradeNames { get; set; }
        public bool IncludeInactiveRegistrations { get; set; }
        public bool MainBranch { get; set; }
        public bool Branch { get; set; }
        public bool LegalPerson { get; set; }
        public int StartPage { get; set; }
    }
}