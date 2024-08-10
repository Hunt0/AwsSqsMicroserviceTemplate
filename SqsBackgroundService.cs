using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Amazon.SQS;
using Amazon.SQS.Model;
using AwsSqsMicroserviceTemplate.Configuration;
using AwsSqsMicroserviceTemplate.Handlers;
using AwsSqsMicroserviceTemplate.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AwsSqsMicroserviceTemplate;

public class SqsBackgroundService : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly ILogger<SqsBackgroundService> _logger;
    private readonly AwsSqsConfiguration _sqsOptions;
    private readonly IServiceProvider _serviceProvider;

    public SqsBackgroundService(
        IAmazonSQS sqs,
        ILogger<SqsBackgroundService> logger,
        IOptionsMonitor<AwsSqsConfiguration> sqsOptionsMonitor,
        IServiceProvider serviceProvider
    )
    {
        _sqs = sqs;
        _logger = logger;
        _sqsOptions = sqsOptionsMonitor.CurrentValue;
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Receiving messages");
            var receiveResponse = await ReceiveMessages(cancellationToken);
            _logger.LogInformation("Finished receiving messages");

            foreach (var message in receiveResponse.Messages)
            {
                var typeName = message.MessageAttributes.GetValueOrDefault("Type")?.StringValue;

                if (typeName == null)
                {
                    _logger.LogError("Received message does not include \"Type\" attribute");
                    continue;
                }

                var handler = GetMessageHandler(typeName);

                if (handler == null)
                {
                    _logger.LogError($"Invalid message type: {typeName}");
                    continue;
                }

                var messageObject = TryDeserialize(message.Body, handler.MessageType);

                if (messageObject == null)
                {
                    _logger.LogError($"Failed to deserialize message of type: {typeName}");
                    continue;
                }

                try
                {
                    _logger.LogInformation("Processing message");
                    handler.ProcessMessage(messageObject);
                    _logger.LogInformation("Finished processing message");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process message");
                }
            }
        }
    }

    private IMessageHandler? GetMessageHandler(string typeName)
    {
        return _serviceProvider
            .GetServices<IMessageHandler>()
            .FirstOrDefault(x => x.MessageType.Name == typeName);
    }
    
    private async Task<ReceiveMessageResponse> ReceiveMessages(CancellationToken cancellationToken)
    {
        var receiveRequest = new ReceiveMessageRequest(_sqsOptions.Url)
        {
            MessageAttributeNames = new List<string>{ "All" }
        };
            
        var receiveResponse = await _sqs.ReceiveMessageAsync(receiveRequest, cancellationToken);

        return receiveResponse;
    }

    private IMessage? TryDeserialize(string body, Type type)
    {
        try
        {
            return (IMessage) JsonSerializer.Deserialize(body, type)!;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}