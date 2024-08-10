using AwsSqsMicroserviceTemplate.Models;
using Microsoft.Extensions.Logging;

namespace AwsSqsMicroserviceTemplate.Handlers;

public class ExampleHandler : IMessageHandler
{
    public Type MessageType => typeof(ExampleMessage);
    private readonly ILogger<ExampleHandler> _logger;

    public ExampleHandler(
        ILogger<ExampleHandler> logger
    )
    {
        _logger = logger;
    }

    public void ProcessMessage(IMessage message)
    {
        var exampleMessage = (ExampleMessage)message;
        _logger.LogInformation($"Name from message: {exampleMessage.Name}");
        _logger.LogInformation($"Value from message: {exampleMessage.Value}");
    }
}