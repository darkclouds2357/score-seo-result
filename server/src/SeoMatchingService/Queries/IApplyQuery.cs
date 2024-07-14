using SeoMatchingService.Domain.DomainEvents;

namespace SeoMatchingService.Queries
{
    public interface IApplyQuery
    {
        Task ApplyAsync(SeoRankSearchedEvent @event, CancellationToken cancellationToken = default);
    }
}