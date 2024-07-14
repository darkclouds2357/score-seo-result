using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

var configuration = GetConfiguration();
var observabilityOptions = new ObservabilityOptions(AppName, EnvironmentName);

Log.Logger = configuration.CreateObservabilityLogger(observabilityOptions);

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", AppName);
    var host = BuildHost(configuration, args)
        .MigrationDbContext();

    Log.Information("Starting web host ({ApplicationContext})...", AppName);
    host.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

IHost BuildHost(IConfiguration configuration, string[] args)
{
    var hostBuilder = Host.CreateDefaultBuilder(args)
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder.CaptureStartupErrors(false)
                           .UseKestrel()
                           .UseStartup<Startup>()
                           .UseConfiguration(configuration)
                           .UseContentRoot(Directory.GetCurrentDirectory())
                           .ConfigureKestrel(serverOptions =>
                           {
                           });
             })
            .UseSerilog(Log.Logger);

    return hostBuilder.Build();
}