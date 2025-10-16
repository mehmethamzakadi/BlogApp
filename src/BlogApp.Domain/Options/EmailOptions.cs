
namespace BlogApp.Domain.Options;

public sealed class EmailOptions
{
    public const string SectionName = "EmailOptions";

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
}
