namespace SeoMatchingService.Dtos
{
    public record SeoRankSearchedHistoryDto
    {
        public required string Id { get; init; }
        public required string SearchedValue { get; init; }
        public required string CompareUrl { get; init; }
        public required string SearchEngine { get; init; }
        public required DateTime SearchedAt { get; init; }
        public required int[] SeoRanks { get; init; } = [];
    }
}