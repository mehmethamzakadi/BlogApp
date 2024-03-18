using BlogApp.Application.Abstractions;
using BlogApp.Application.Features.Authorizations.Commands.UserLogin;
using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BlogApp.Application.Services;

public sealed class TokenService(UserManager<AppUser> userManager, IConfiguration configuration) : ITokenService
{
    public TokenResponse GenerateAccessToken(IEnumerable<Claim> claims, AppUser user)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenOptions:SecurityKey"] ?? string.Empty)), SecurityAlgorithms.HmacSha256);

        var issuer = configuration["TokenOptions:Issuer"];
        var audience = configuration["TokenOptions:Audience"];
        var expires = Convert.ToInt32(configuration["TokenOptions:AccessTokenExpiration"]);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            expires: DateTime.Now.AddMinutes(expires),
            claims: claims,
            signingCredentials: signingCredentials
            );

        var refreshToken = GenerateRefreshToken();
        var tokenResponse = new TokenResponse(
            UserId: user.Id,
            UserName: user.UserName ?? string.Empty,
            Expiration: token.ValidTo,
            Token: new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken: refreshToken);

        return tokenResponse;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<List<Claim>> GetAuthClaims(AppUser user)
    {
        var userRoles = await userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
                    {
                        new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new (ClaimTypes.Email, user.Email ?? string.Empty),
                        new (ClaimTypes.Name, user.UserName ?? string.Empty),
                        new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        return authClaims;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenOptions:SecurityKey"] ?? string.Empty)),
            ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }
}
