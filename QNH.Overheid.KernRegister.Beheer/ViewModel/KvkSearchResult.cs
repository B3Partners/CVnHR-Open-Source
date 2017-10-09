using System;
using System.Collections.Generic;

namespace QNH.Overheid.KernRegister.Beheer.ViewModels
{
    public class KvkSearchResult
    {
        public List<KvkItem> KvkItems { get; set; }
        public bool SearchedAndNothingFound { get; set; }
        public int TotalFound { get; set; }
        public KvkSearchResult()
        {
            KvkItems = new List<KvkItem>();
            SearchedAndNothingFound = false;
        }

        public Exception SearchError { get; set; }
        
    }
}


