using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using QNH.Overheid.KernRegister.Business.Model.Entities;

namespace QNH.Overheid.KernRegister.Business.Model.Debug
{
    public class DatabaseDebugInfo
    {
        public IEnumerable<KvkInschrijving> InschrijvingOrphans { get; set; }
        public IEnumerable<Vestiging> VestigingOrphans { get; set; }

        public IEnumerable<DeponeringsStuk> DeponeringsStukOrphans { get; set; }
        public IEnumerable<HandelsNaam> HandelsnaamOrphans { get; set; }
        public IEnumerable<FunctieVervulling> FunctieVervullingOrphans { get; set; }
        public IEnumerable<SbiActiviteit> SbiActiviteitOrphans { get; set; }
        public IEnumerable<SbiCode> SbiCodeOrphans { get; set; }
        public IEnumerable<VestigingSbiActiviteit> VestigingSbiActiviteitOrphans { get; set; }

        public int ExpiredInschrijvingenCount { get; set; }
        public int ExpiredVestigingenCount { get; set; }

        public IEnumerable<Vestiging> DoubleVestigingen { get; set; }

        public Exception Exception { get; set; }
    }
}
