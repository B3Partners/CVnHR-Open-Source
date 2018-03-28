using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.Business.Service
{
    public interface IKvkSearchServiceSelector
    {
        string GetConfigVersion();
        IKvkSearchService GetService(string version);
    }

    public class KvkSearchServiceSelector : IKvkSearchServiceSelector
    {
        private readonly string _configVersion;
        private readonly Dictionary<string, IKvkSearchService> _searchServices;

        public KvkSearchServiceSelector(string configVersion, Dictionary<string, IKvkSearchService> availableServices)
        {
            _configVersion = configVersion;
            _searchServices = availableServices;
        }

        public string GetConfigVersion() => _configVersion;

        public IKvkSearchService GetService(string version) => _searchServices[version];
    }
}
