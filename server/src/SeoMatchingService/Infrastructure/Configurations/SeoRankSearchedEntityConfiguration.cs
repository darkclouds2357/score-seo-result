using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using SeoMatchingService.Infrastructure.Entities;

namespace SeoMatchingService.Infrastructure.Configurations
{
    public partial class SeoRankSearchedEntityConfiguration : IEntityTypeConfiguration<SeoRankSearchedEntity>
    {
        public void Configure(EntityTypeBuilder<SeoRankSearchedEntity> entityBuilder)
        {
            entityBuilder.ToTable(SeoRankSearchedEntity.TABLE_NAME, action => action.IsMemoryOptimized());

            entityBuilder.HasKey(t => t.Id);

            entityBuilder.Property(e => e.Id).HasColumnName("id");
            entityBuilder.Property(e => e.SearchedValue).HasColumnName("searched_value");
            entityBuilder.Property(e => e.SearchEngine).HasColumnName("search_engine");
            entityBuilder.Property(e => e.SearchedAt).HasColumnName("searched_at");
            entityBuilder.Property(e => e.CompareUrl).HasColumnName("compared_url");
            entityBuilder.Property(e => e.SeoRanks)
                .HasColumnName("seo_ranks")
                .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<int[]>(v));

            entityBuilder.HasIndex(e => e.SearchedAt);
            entityBuilder.HasIndex(e => e.SearchEngine);
            entityBuilder.HasIndex(e => new { e.SearchedAt, e.SearchEngine });
        }
    }
}