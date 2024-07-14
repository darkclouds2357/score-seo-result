using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;

namespace SeoMatchingService.SearchEngine
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSearchEngines(this IServiceCollection services, IConfiguration configuration, params Assembly[] engineAssemblies)
        {
            engineAssemblies ??= [];
            foreach (var engineAssembly in engineAssemblies)
            {
                var type = engineAssembly.GetTypes().FirstOrDefault(t => t.Name == nameof(ServiceCollectionExtension));
                if (type == null)
                    continue;

                var addProviderMethod = type.GetMethod("AddSearchEngine", BindingFlags.Static | BindingFlags.Public);

                if (addProviderMethod == null)
                    continue;

                var methodDelegate = (Func<IServiceCollection, IConfiguration, IServiceCollection>)Delegate.CreateDelegate(typeof(Func<IServiceCollection, IConfiguration, IServiceCollection>), addProviderMethod);

                services = methodDelegate(services, configuration);
            }

            var config = new CrawlerParserEngineConfig();
            configuration.Bind(config);

            services.AddScoped<CrawlerManager>();

            services.AddSingleton(sp =>
            {
                var logger = sp.GetService<ILogger<CrawlerParser>>();
                return new CrawlerParser(config.SearchEnginePatterns, logger);
            });

            return services;
        }
    }
}