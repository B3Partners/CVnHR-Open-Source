using Newtonsoft.Json;
using QNH.Overheid.KernRegister.Business.Enums;
using System;
using System.Collections.Generic;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    public class BrmoConfig
    {
        public string PostCodes { get; set; }

        public string taskName { get; set; }
        
        public string HRDataserviceVersion { get; set; } = "3.0";

        public BrmoProcessTypes BrmoProcessType { get; set; } = BrmoProcessTypes.ZipCodes;

        [JsonIgnore]
        public IEnumerable<string> PossibleHRDataserviceVersions { get; set; } = new[] { "3.0" };

        [JsonIgnore]
        public IEnumerable<string> PossibleBrmoProcessTypes { get { return Enum.GetNames(typeof(BrmoProcessTypes)); } }

    }
}