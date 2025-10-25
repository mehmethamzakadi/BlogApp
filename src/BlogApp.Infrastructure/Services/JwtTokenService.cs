using BlogApp.Application.Abstractions.Identity;
using BlogApp.Application.Features.Auths.Login;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TokenOptions = BlogApp.Domain.Options.TokenOptions;

namespace BlogApp.Infrastructure.Services.Identity;

public sealed class JwtTokenService : ITokenService
{
    private readonly IUserRepository _userRepository;
    private readonly BlogAppDbContext _dbContext;
    private readonly IPermissionRepository _permissionRepository;
    private readonly TokenOptions _tokenOptions;

    public JwtTokenService(
        IUserRepository userRepository,
        BlogAppDbContext dbContext,
        IPermissionRepository permissionRepository,
        IOptions<TokenOptions> tokenOptionsAccessor)
    {
        _userRepository = userRepository;
        _dbContext = dbContext;
        _permissionRepository = permissionRepository;
        _tokenOptions = tokenOptionsAccessor.Value;
    }

    public LoginResponse GenerateAccessToken(IEnumerable<Claim> claims, User user)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecurityKey)),
            SecurityAlgorithms.HmacSha256);

        DateTime expiration = DateTime.UtcNow.AddMinutes(_tokenOptions.AccessTokenExpiration);

        var token = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            expires: expiration,
            claims: claims,
            signingCredentials: signingCredentials);

        var refreshToken = GenerateRefreshToken();

        // Extract permissions from claims
        var permissions = claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();

        var tokenResponse = new LoginResponse(
            UserId: user.Id,
            UserName: user.UserName,
            Expiration: token.ValidTo,
            Token: new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken: refreshToken,
            Permissions: permissions);

        return tokenResponse;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<List<Claim>> GetAuthClaims(User user)
    {
        var userRoles = await _userRepository.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.UserName),
        };

        // Add role claims
        foreach (var roleName in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, roleName));
        }

        // Add permission claims
        if (userRoles.Any())
        {
            // Get all role IDs
            var roleIds = await _dbContext.Roles
                .Where(r => userRoles.Contains(r.Name))
                .Select(r => r.Id)
                .ToListAsync();

            if (roleIds.Any())
            {
                var permissions = await _permissionRepository.GetPermissionsByRoleIdsAsync(roleIds);
                foreach (var permission in permissions)
                {
                    authClaims.Add(new Claim("permission", permission.Name));
                }
            }
        }

        return authClaims;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecurityKey)),
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }
        return principal;
    }
}
