namespace BlogApp.Application.Features.Auths.Login;

public sealed record LoginResponse(
    Guid UserId,
    string UserName,
    DateTime Expiration,
    string Token,
    string RefreshToken,
    List<string> Permissions);
