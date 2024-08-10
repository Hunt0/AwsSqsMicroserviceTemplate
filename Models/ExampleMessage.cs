namespace AwsSqsMicroserviceTemplate.Models;

public class ExampleMessage : IMessage
{
    public string Name { get; set; } = default!;

    public string Value { get; set; } = default!;
}