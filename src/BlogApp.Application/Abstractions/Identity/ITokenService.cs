using BlogApp.Domain.Entities;
using System.Security.Claims;

namespace BlogApp.Application.Abstractions.Identity;

public interface ITokenService
{
    AccessTokenResult CreateAccessToken(IEnumerable<Claim> claims, User user);
    RefreshTokenResult CreateRefreshToken();
    Task<List<Claim>> GetAuthClaims(User user);
}

public sealed record AccessTokenResult(string Token, DateTime ExpiresAt, Guid Jti, IReadOnlyList<string> Permissions);

public sealed record RefreshTokenResult(string Token, DateTime ExpiresAt);
