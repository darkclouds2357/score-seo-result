using Microsoft.EntityFrameworkCore;
using SeoMatchingService.Infrastructure.Entities;

namespace SeoMatchingService.Infrastructure
{
    public class SeoRankingDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<SeoRankSearchedEntity> SeoRankSearcheds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }

        public virtual void Migrate() => Database.Migrate();

    }
}