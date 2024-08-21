using BlogApp.Application.Features.AppUsers.Commands.Login;
using BlogApp.Domain.Entities;
using System.Security.Claims;

namespace BlogApp.Application.Abstractions.Identity;

public interface ITokenService
{
    LoginResponse GenerateAccessToken(IEnumerable<Claim> claims, AppUser user);
    string GenerateRefreshToken();
    Task<List<Claim>> GetAuthClaims(AppUser user);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
