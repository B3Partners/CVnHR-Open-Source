using Newtonsoft.Json;
using QNH.Overheid.KernRegister.Business.KvKSearchApi.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace QNH.Overheid.KernRegister.Business.KvKSearchApi
{
    public class KvkSearchApi : IKvkSearchApi
    {
        public readonly string _baseUrl;
        public readonly string _searchUrl;
        public readonly string _apiKey;

        public KvkSearchApi(string baseUrl, string searchUrl, string apiKey)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _searchUrl = searchUrl.TrimStart('/');
            _apiKey = apiKey;
        }

        public string FormattedQueryString => $"{_baseUrl}/{_searchUrl}?apiKey={HttpUtility.UrlEncode(_apiKey)}";

        public async Task<KvkSearchApiResult> Search(KvkSearchApiParameters parameters)
        {
            var httpClient = new HttpClient();

            var search = new List<string>();
            search.Add(FormattedQueryString);
            if (!string.IsNullOrWhiteSpace(parameters.Q))
            {
                search.Add($"q={parameters.Q}");
            }
            if (parameters.StartPage > 0)
            {
                search.Add($"startpage={parameters.StartPage}");
            }
            // TODO: add other parameters

            var url = string.Join("&", search);
            var result = await httpClient.GetAsync(url);
            result.EnsureSuccessStatusCode();
            var content = await result.Content.ReadAsStringAsync();

            var apiResult = JsonConvert.DeserializeObject<KvkSearchApiResultWrapper>(content);

            return apiResult.Data;
        }
    }
}
