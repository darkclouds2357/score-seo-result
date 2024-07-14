namespace SeoMatchingService.Infrastructure.Entities
{
    public class SeoRankSearchedEntity
    {
        public const string TABLE_NAME = "seo_rank_searched";
        public string Id { get; set; }

        public string SearchedValue { get; set; }

        public string CompareUrl { get; set; }

        public string SearchEngine { get; set; }
        public DateTime SearchedAt { get; set; }
        public int[] SeoRanks { get; set; } = [];
    }
}