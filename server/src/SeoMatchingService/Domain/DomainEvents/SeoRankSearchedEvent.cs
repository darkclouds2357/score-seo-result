using SeoMatchingService.Queries;

namespace SeoMatchingService.Domain.DomainEvents
{
    public record SeoRankSearchedEvent : INotification
    {
        public required string ComparedUrl { get; init; }
        public required string SearchedValue { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required int SampleCount { get; init; }
        public required IReadOnlyDictionary<string, SearchResult[]> SeoSearchedResults { get; init; }
    }

    public class SeoRankSearchedEventHandler : INotificationHandler<SeoRankSearchedEvent>
    {
        private readonly IApplyQuery _applyQuery;

        public SeoRankSearchedEventHandler(IApplyQuery applyQuery)
        {
            _applyQuery = applyQuery;
        }

        public Task Handle(SeoRankSearchedEvent @event, CancellationToken cancellationToken)
        {
            return _applyQuery.ApplyAsync(@event, cancellationToken);
        }
    }
}