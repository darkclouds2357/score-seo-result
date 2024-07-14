using System.Threading;
using System.Threading.Tasks;

namespace SeoMatchingService.SearchEngine
{
    public interface ISearchEngineCrawler
    {
        string Name { get; }

        Task<CrawlResult[]> CrawlAsync(string searchValue, int count, CancellationToken cancellationToken = default);
    }
}