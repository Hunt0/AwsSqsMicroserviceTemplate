using AwsSqsMicroserviceTemplate.Models;

namespace AwsSqsMicroserviceTemplate.Handlers;

public interface IMessageHandler
{
    public Type MessageType { get; }
    void ProcessMessage(IMessage message);
}