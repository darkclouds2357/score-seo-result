using SeoMatchingService.Dtos;

namespace SeoMatchingService.Queries
{
    public interface ISeoRankSearchedQuery
    {
        Task<IReadOnlyList<SeoRankSearchedHistoryDto>> GetSeoRankSearchedHistoriesAsync(string searchEnigne = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    }
}