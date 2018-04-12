using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using NHibernate.Util;
using NLog;

namespace QNH.Overheid.KernRegister.Business.Business
{
    public static class RawXmlCache
    {
        public static int CacheInHours { get; set; }

        private static readonly MemoryCache _cache = MemoryCache.Default;// new MemoryCache(nameof(RawXmlCache));

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static void Add(XDocument xDoc)
        {
            var kvkNummer = xDoc.Descendants().FirstOrDefault(node => node.Name.LocalName == "kvkNummer")?.Value 
                            ?? xDoc.Descendants().FirstOrDefault(node => node.Name.LocalName == "referentie")?.Value;
            if (kvkNummer == null)
            {
                logger.Warn("Could not cache xDocument because no kvkNummer was found");
                logger.Info(xDoc.ToString());
                return;
            }
            if (!_cache.Contains(kvkNummer))
            {
                _cache.Add(kvkNummer, xDoc, DateTimeOffset.Now.AddHours(CacheInHours));
            }
            else
            {
                _cache[kvkNummer] = xDoc;
            }
        }

        public static int GetCount() => _cache.Count();

        public static XDocument Get(string kvkNummer, Action retryGet = null)
        {
            Func<MemoryCache, XDocument> xDocGetter = (cache) => cache.Contains(kvkNummer) 
                    ? (XDocument)cache.Get(kvkNummer) 
                    : (XDocument)cache.FirstOrDefault(k => k.Key.Contains(kvkNummer)).Value;
            var xDoc = xDocGetter(_cache);
            if (xDoc == null)
            {
                retryGet?.Invoke();
                xDoc = xDocGetter(_cache);
            }
            return xDoc;
        }
    }
}
