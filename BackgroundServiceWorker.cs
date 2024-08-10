using Microsoft.Extensions.Hosting;
using Amazon.SQS;
using Microsoft.Extensions.Logging;

namespace AwsSqsMicroserviceTemplate;

public class BackgroundServiceWorker : BackgroundService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger<BackgroundServiceWorker> _logger;
    
    public BackgroundServiceWorker(
        IAmazonSQS sqsClient,
        ILogger<BackgroundServiceWorker> logger
    )
    {
        _sqsClient = sqsClient;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("STARTED receiving message");
            var message = await _sqsClient.ReceiveMessageAsync("url", cancellationToken);
            _logger.LogInformation("FINISHED receiving message");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FAILED to receive message"); 
        }
    }
}