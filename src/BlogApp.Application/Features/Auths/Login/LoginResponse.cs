namespace BlogApp.Application.Features.Auths.Login;

public sealed record LoginResponse(
    int UserId,
    string UserName,
    DateTime Expiration,
    string Token,
    string RefreshToken);