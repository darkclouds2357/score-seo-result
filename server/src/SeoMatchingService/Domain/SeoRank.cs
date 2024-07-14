using SeoMatchingService.Domain.DomainEvents;
using SeoMatchingService.SearchEngine;

namespace SeoMatchingService.Domain
{
    public class SeoRank(CrawlerManager crawlerManager, IMediator mediator)
    {

        private const int CrawlerCount = 100; // TODO this one should move to config

        private readonly CrawlerManager _crawlerManager = crawlerManager;
        private readonly IMediator _mediator = mediator;
        private List<SearchResult> _engineSearchedResults = [];

        private Uri _comparedUri => new(ComparedUrl);

        public IReadOnlyList<SearchResult> EngineSearchedResults => _engineSearchedResults;

        public string ComparedUrl { get; private set; }
        public string SearchValue { get; private set; }
        public DateTime SearchedAt { get; private set; }

        private IReadOnlyDictionary<string, SearchResult[]> _seoSearchedResult => _engineSearchedResults.GroupBy(c => c.SearchEngine).ToDictionary(r => r.Key, r => r.Where(c => c.Uri?.Host == _comparedUri.Host).ToArray());

        public IReadOnlyDictionary<string, int[]> SearchedRanks => _seoSearchedResult.ToDictionary(c => c.Key, v => v.Value.Select(s => s.Order).ToArray());

        public async Task<IReadOnlyDictionary<string, int[]>> SeoRankingAsync(string searchValue, string compareUrl, CancellationToken cancellationToken = default)
        {
            return await this.BuildRanking(searchValue, compareUrl).SearchAsync(cancellationToken);
        }

        private SeoRank BuildRanking(string searchValue, string comparedUrl)
        {
            ComparedUrl = comparedUrl;
            SearchValue = searchValue;

            return this;
        }

        private async Task<IReadOnlyDictionary<string, int[]>> SearchAsync(CancellationToken cancellationToken = default)
        {
            _engineSearchedResults = [];
            await foreach (var item in _crawlerManager.CrawlAsync(SearchValue, CrawlerCount, cancellationToken))
            {
                _engineSearchedResults.AddRange(item.Select(v => new SearchResult(SearchValue, v)));
            }
            SearchedAt = DateTime.UtcNow;

            await _mediator.Publish(new SeoRankSearchedEvent
            {
                ComparedUrl = ComparedUrl,
                CreatedAt = SearchedAt,
                SeoSearchedResults = _seoSearchedResult,
                SampleCount = CrawlerCount,
                SearchedValue = SearchValue
            }, cancellationToken);

            return SearchedRanks;
        }
    }
}