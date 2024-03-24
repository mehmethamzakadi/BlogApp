namespace BlogApp.Application.Features.AppUsers.Commands.Login;

public sealed record LoginResponse(
    int UserId,
    string UserName,
    DateTime Expiration,
    string Token,
    string RefreshToken);