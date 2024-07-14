using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SeoMatchingService.SearchEngine.Google
{
    public class GoogleSearchClient : ISearchEngineCrawler
    {
        private const string _searchUrl = "https://www.google.com/search?num={count}&q={query}";

        private readonly HttpClient _httpClient;
        private readonly CrawlerParser _crawlerXMLParser;
        private readonly ILogger<GoogleSearchClient> _logger;

        public GoogleSearchClient(HttpClient httpClient, CrawlerParser crawlerXMLParser, ILogger<GoogleSearchClient> logger)
        {
            _httpClient = httpClient;
            _crawlerXMLParser = crawlerXMLParser;
            this._logger = logger;
        }

        public string Name => "Google";

        public async Task<CrawlResult[]> CrawlAsync(string searchValue, int count, CancellationToken cancellationToken = default)
        {
            var content = await GetSourceResultAsync(searchValue, count, cancellationToken);

            var result = _crawlerXMLParser.HTMLContentParser(Name, content);

            if (result.Length == 0)
            {
                return [new CrawlResult(Name, null, null, null, 0)];
            }

            return result;
        }

        private async Task<string> GetSourceResultAsync(string searchValue, int count, CancellationToken cancellationToken = default)
        {
            var searchUrl = _searchUrl.Replace("{query}", searchValue.Replace(" ", "+"))
                                    .Replace("{count}", count.ToString());

            try
            {
                using HttpRequestMessage request = new();

                request.Method = new HttpMethod("GET");

                request.RequestUri = new Uri(searchUrl, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync(cancellationToken);
                }
                else
                {
                    _logger.LogWarning("Return Failed From Google. Status Code {code}", response.StatusCode);
                    throw new Exception($"Return Failed From Google. Status Code {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return string.Empty;
        }
    }
}