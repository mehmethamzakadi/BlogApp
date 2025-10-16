
namespace BlogApp.Domain.Options;

public sealed class TokenOptions
{
    public const string SectionName = "TokenOptions";

    public string Audience { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public int AccessTokenExpiration { get; set; }
    public string SecurityKey { get; set; } = string.Empty;
}
