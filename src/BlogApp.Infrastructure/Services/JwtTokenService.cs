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

    public AccessTokenResult CreateAccessToken(IEnumerable<Claim> claims, User user)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecurityKey)),
            SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        DateTime expiration = now.AddMinutes(_tokenOptions.AccessTokenExpiration);
        var claimsList = claims.ToList();
        var jti = Guid.NewGuid();

        claimsList.Add(new Claim(JwtRegisteredClaimNames.Jti, jti.ToString()));
        claimsList.Add(new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64));

        var token = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            expires: expiration,
            claims: claimsList,
            signingCredentials: signingCredentials);

        var permissions = claimsList
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();

        return new AccessTokenResult(
            new JwtSecurityTokenHandler().WriteToken(token),
            token.ValidTo,
            jti,
            permissions);
    }

    public RefreshTokenResult CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var token = Convert.ToBase64String(randomNumber);
        var expiresAt = DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenExpirationDays);
        return new RefreshTokenResult(token, expiresAt);
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

}
