namespace QNH.Overheid.KernRegister.Beheer.ViewModels
{
    public class KvkSearch
    {
        public KvkSearchCriteria KvkSearchCriteria { get; set; }
        public KvkSearchResult KvkSearchResult { get; set; }

        public KvkSearch()
        {
            KvkSearchCriteria = new KvkSearchCriteria();
            KvkSearchResult = new KvkSearchResult();
        }
    }
}