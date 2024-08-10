namespace AwsSqsMicroserviceTemplate.Configuration;

public class AwsSqsConfiguration
{
    public const string Section = "AwsSqs";
    public const string SectionAccessKey = "AwsSqs:AccessKey";
    public const string SectionSecretKey = "AwsSqs:SecretKey";

    public string Url { get; set; } = default!;
    public string AccessKey { get; set; } = default!;
    public string SecretKey { get; set; } = default!;
}