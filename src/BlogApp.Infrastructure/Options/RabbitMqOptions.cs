namespace BlogApp.Infrastructure.Options;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMQOptions";

    public string HostName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int RetryLimit { get; set; }
}
