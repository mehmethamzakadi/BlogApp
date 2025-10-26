using System.Text.Json.Serialization;

namespace BlogApp.Application.Features.Auths.Login;

public sealed record LoginResponse(
    Guid UserId,
    string UserName,
    DateTime Expiration,
    string Token,
    [property: JsonIgnore] string RefreshToken,
    List<string> Permissions);
