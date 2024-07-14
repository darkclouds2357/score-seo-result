using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SeoMatchingService.SearchEngine.Google
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSearchEngine(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<ISearchEngineCrawler, GoogleSearchClient>("Google", config =>
            {
                config.BaseAddress = new Uri("https://www.google.com/");
            });

            return services;
        }
    }
}