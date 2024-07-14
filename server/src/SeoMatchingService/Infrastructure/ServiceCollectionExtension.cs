using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SeoMatchingService.Infrastructure.Query;
using SeoMatchingService.Queries;

namespace SeoMatchingService.Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<RelationalDatabaseQuery>();

            services.AddScoped<IApplyQuery>(sp => sp.GetRequiredService<RelationalDatabaseQuery>());
            services.AddScoped<ISeoRankSearchedQuery>(sp => sp.GetRequiredService<RelationalDatabaseQuery>());

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddEntityFrameworkSqlServer();
            services.AddDbContext<SeoRankingDbContext>((sp, builder) =>
            {
                builder.CreateDbContextOptionsBuilder<SeoRankingDbContext>(connectionString);
                builder.UseInternalServiceProvider(sp);
            });

            return services;
        }



        public static DbContextOptionsBuilder CreateDbContextOptionsBuilder<TContext>(this DbContextOptionsBuilder options, string connectionString, string assemblyName = null)
            where TContext : DbContext
        {
            if (string.IsNullOrWhiteSpace(assemblyName))
                assemblyName = typeof(TContext).Assembly.FullName;


            options.UseSqlServer(connectionString, option =>
            {
                option.MigrationsAssembly(assemblyName);
                option.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromMilliseconds(100), errorNumbersToAdd: null);
            });

            return options;
        }
    }
}