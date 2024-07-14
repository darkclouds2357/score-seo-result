using Microsoft.Extensions.Hosting;
using SeoMatchingService.Infrastructure;

public static class HostExtension
{
    public static IHost MigrationDbContext(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<SeoRankingDbContext>();

        dbContext.Migrate();

        return host;
    }
}