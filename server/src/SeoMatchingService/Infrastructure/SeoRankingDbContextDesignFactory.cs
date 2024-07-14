using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SeoMatchingService.Infrastructure
{
    public class SeoRankingDbContextDesignFactory : IDesignTimeDbContextFactory<SeoRankingDbContext>
    {
        public SeoRankingDbContext CreateDbContext(string[] args) => new(GetDbContextOptions(args));

        private static DbContextOptions GetDbContextOptions(string[] args)
        {
            var connectionString = string.Empty;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--connectionString":
                    case "-con":
                        connectionString = args[++i];
                        break;

                    default:
                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("Missing connection string for migration");

            var optionsBuilder = new DbContextOptionsBuilder<SeoRankingDbContext>();

            optionsBuilder.CreateDbContextOptionsBuilder<SeoRankingDbContext>(connectionString);

            return optionsBuilder.Options;
        }
    }
}