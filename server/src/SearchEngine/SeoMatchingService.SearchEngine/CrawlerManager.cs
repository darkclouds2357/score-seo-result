using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SeoMatchingService.SearchEngine
{
    public record CrawlResult(string Enigne, string Url, string Title, string Snippets, int Order);

    public class CrawlerManager(IEnumerable<ISearchEngineCrawler> crawlers)
    {
        private List<ISearchEngineCrawler> _engines = [.. crawlers];

        public async IAsyncEnumerable<CrawlResult[]> CrawlAsync(string searchValue,int count, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _engines ??= [];
            foreach (var engine in _engines)
            {
                yield return await engine.CrawlAsync(searchValue, count, cancellationToken);
            }
        }
    }
}