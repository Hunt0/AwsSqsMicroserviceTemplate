using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SQS;
using AwsSqsMicroserviceTemplate.Configuration;
using AwsSqsMicroserviceTemplate.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AwsSqsMicroserviceTemplate;

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

            services.AddHostedService<SqsBackgroundService>();

            var sqsConfiguration = hostContext.Configuration.GetSection(AwsSqsConfiguration.Section);
            services.Configure<AwsSqsConfiguration>(sqsConfiguration);
            
            //NOTE: A production application should use temporary keys via IAM roles on running instances
            var awsOptions = new AWSOptions()
            {
                Region = RegionEndpoint.USEast2,
                Credentials = new BasicAWSCredentials(
                    hostContext.Configuration[AwsSqsConfiguration.SectionAccessKey],
                    hostContext.Configuration[AwsSqsConfiguration.SectionSecretKey]
                )
            };
            
            services.AddAWSService<IAmazonSQS>(awsOptions);

            services.AddSingleton<IMessageHandler, ExampleHandler>();
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