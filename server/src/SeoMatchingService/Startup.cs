﻿using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SeoMatchingService.Domain;
using SeoMatchingService.Infrastructure;
using SeoMatchingService.SearchEngine;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;
using System.Reflection;

namespace SeoMatchingService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            AddMeditor(services);
            AddCorsService(services, Configuration);
            AddApiVersioningService(services);

            AddOptionConfigurationServices(services);
            AddSwaggerGenService(services, this.GetType().Assembly);

            services.AddControllers();
            services
                .AddMvcCore(option => { });
            //.AddNewtonsoftJson(options => { });
            //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddHealthChecks();
            services.AddRazorPages();

            services.AddScoped<SeoRank>();

            services.AddDatabase(Configuration);

            services.AddSearchEngines(Configuration, Env.SearchEngineAssembly);
        }

        private IServiceCollection AddMeditor(IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly));

            return services;
        }

        private IServiceCollection AddApiVersioningService(IServiceCollection services)
        {
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.DefaultApiVersion = new ApiVersion(1, 0); // specify the default api version
                o.AssumeDefaultVersionWhenUnspecified = true; // assume that the caller wants the default version if they don't specify
                o.ApiVersionReader = new MediaTypeApiVersionReader("api-version"); // read the version number from the accept header
            }).AddApiExplorer(o =>
            {
                o.GroupNameFormat = "'V'VVV";
            });
            return services;
        }

        private IServiceCollection AddCorsService(IServiceCollection services, IConfiguration configuration)
        {
            var domains = configuration["Cors:Domains"].Split(',').ToArray();
            if (domains.Length > 0)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder
                        .SetIsOriginAllowed((host) => true)
                        .WithOrigins(domains)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
                });
            }
            else
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
                });
            }

            return services;
        }

        private IServiceCollection AddOptionConfigurationServices(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details."
                    };

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json", "application/problem+xml" }
                    };
                };
            });

            return services;
        }

        private IServiceCollection AddSwaggerGenService(IServiceCollection services, Assembly assembly)
        {
            services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider()
                    .GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var apiVersion in provider.ApiVersionDescriptions)
                {
                    // ConfigureVersionedDescription(options, apiVersion);
                    options.SwaggerDoc(apiVersion.GroupName, new OpenApiInfo()
                    {
                        Title = $"API - version {apiVersion.ApiVersion}",
                        Version = apiVersion.ApiVersion.ToString(),
                        Description = apiVersion.IsDeprecated ? $"API - DEPRECATED" : "API",
                    });
                }
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "Bearer",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
                var servicePrefix = Environment.GetEnvironmentVariable("SERVICE_PREFIX");
                if (!string.IsNullOrWhiteSpace(servicePrefix))
                {
                    options.DocumentFilter<ServicePrefixInsertDocumentFilter>(servicePrefix);
                }

                //// Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath);
            });

            return services;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            UseCorsz(app);
            UseSwaggerz(app, provider);

            var healthCheckPattern = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HEALTH_CHECK_ENDPOINT")) ? Environment.GetEnvironmentVariable("HEALTH_CHECK_ENDPOINT") : "/ping";
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks(healthCheckPattern);
            });
        }

        public void UseCorsz(IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");
        }

        public void UseSwaggerz(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var apiVersion in provider.ApiVersionDescriptions
                    .OrderBy(version => version.ToString()))
                {
                    c.SwaggerEndpoint(
                        $"swagger/{apiVersion.GroupName.ToUpperInvariant()}/swagger.json", apiVersion.GroupName.ToUpperInvariant()
                    );
                }

                c.RoutePrefix = string.Empty;
            });
        }
    }
}

public class ServicePrefixInsertDocumentFilter : IDocumentFilter
{
    private readonly string _prefix;

    public ServicePrefixInsertDocumentFilter(string prefix)
    {
        _prefix = prefix;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = swaggerDoc.Paths.Keys.ToList();
        foreach (var path in paths)
        {
            var partPrefix = $"/{_prefix}{path}";

            if (!path.StartsWith("/"))
                partPrefix = $"/{_prefix}/{path}";

            var pathToChange = swaggerDoc.Paths[path];
            swaggerDoc.Paths.Remove(path);
            swaggerDoc.Paths.Add(partPrefix, pathToChange);
        }
    }
}