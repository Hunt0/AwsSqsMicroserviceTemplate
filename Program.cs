
using System.Runtime.InteropServices;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SQS;
using AwsSqsMicroserviceTemplate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

public class Program
{
    static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var hostBuilder = Host.CreateDefaultBuilder(args);

        hostBuilder.ConfigureAppConfiguration((hostContext, config) =>
        {
            config.SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile(
                    path: "appsettings.json",
                    optional: false,
                    reloadOnChange: true
                )
                .AddJsonFile(
                    path: $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                    optional: true,
                    reloadOnChange: true
                );
        });

        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            services.AddOptions();

            services.AddHostedService<BackgroundServiceWorker>();

            services.AddAWSService<IAmazonSQS>();

        });

        hostBuilder.UseSerilog((_, _, logConfig) =>
        {
            logConfig
                .WriteTo.Console()
                .MinimumLevel.Information();
        });
        
        return hostBuilder;
    }
}





