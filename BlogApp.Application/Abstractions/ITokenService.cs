using BlogApp.Application.Features.Authorizations.Commands.UserLogin;
using BlogApp.Domain.Entities;
using System.Security.Claims;

namespace BlogApp.Application.Abstractions;

public interface ITokenService
{
    TokenResponse GenerateAccessToken(IEnumerable<Claim> claims, AppUser user);
    string GenerateRefreshToken();
    Task<List<Claim>> GetAuthClaims(AppUser user);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
