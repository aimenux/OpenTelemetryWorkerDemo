using App.Extensions;
using App.Helpers;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace App;

public static class Program
{
    public static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();
        var configuration = host.Services.GetRequiredService<IConfiguration>();
        using var _ = OpenTelemetryBootstrapper.CreateOpenTelemetryTracer(configuration);
        await host.RunAsync();

        Console.WriteLine("Press any key to exit !");
        Console.ReadKey();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddJsonFile();
                config.AddEnvironmentVariables();
                config.AddCommandLine(args);
            })
            .ConfigureLogging((hostingContext, loggingBuilder) =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsoleLogger();
                loggingBuilder.AddNonGenericLogger();
                loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection(@"Logging"));
                loggingBuilder.AddOpenTelemetry(options =>
                {
                    options
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("OpenTelemetryLogger"))
                        .AddConsoleExporter();
                });
            })
            .ConfigureServices((_, services) =>
            {
                services.AddHttpClient<Proxy>();
                services.AddHostedService<Worker>();
            })
            .UseConsoleLifetime()
            .UseWindowsService()
            .UseSystemd();
}