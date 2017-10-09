using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.Business.KvKSearchApi.Entities
{
    public class KvkSearchApiParameters
    {
        public string q { get; set; }
        public string kvkNumber { get; set; }
        public string branchNumber { get; set; }
        public string Rsin { get; set; }
        public string tradeName { get; set; }
        public string Street { get; set; }
        public string houseNumber { get; set; }
        public string Postalcode { get; set; }
        public string City { get; set; }
        public bool includeFormerTradeNames { get; set; }
        public bool includeInactiveRegistrations { get; set; }
        public bool mainBranch { get; set; }
        public bool branch { get; set; }
        public bool legalPerson { get; set; }
        public int startPage { get; set; }
    }

    public class KvkSearchApiResult
    {
        public IEnumerable<dynamic> ResultsJson;
    }
}
