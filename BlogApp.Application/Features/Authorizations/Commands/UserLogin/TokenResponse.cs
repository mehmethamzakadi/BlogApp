namespace BlogApp.Application.Features.Authorizations.Commands.UserLogin;

public sealed record TokenResponse(
    int UserId,
    string UserName,
    DateTime Expiration,
    string Token,
    string RefreshToken);