using BlogApp.Application.DTOs.AppUsers;
using BlogApp.Application.Utilities.Results;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogApp.Application.Features.Authorizations.Queries
{
    public class UserLoginQuery : IRequest<IDataResult<TokenResponse>>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }

        public class RegisterQueryHandler : IRequestHandler<UserLoginQuery, IDataResult<TokenResponse>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly SignInManager<AppUser> _signInManager;

            private readonly IConfiguration Configuration;

            public RegisterQueryHandler(UserManager<AppUser> userManager, IConfiguration configuration, SignInManager<AppUser> signInManager)
            {
                _userManager = userManager;
                Configuration = configuration;
                _signInManager = signInManager;
            }

            public async Task<IDataResult<TokenResponse>> Handle(UserLoginQuery request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }
                    await _signInManager.SignInWithClaimsAsync(user, false, authClaims);

                    var token = GetToken(authClaims);
                    var result = new TokenResponse
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Expiration = token.ValidTo
                    };

                    return new SuccessDataResult<TokenResponse>(result, "Giriş Başarılı");
                }
                return new ErrorDataResult<TokenResponse>("E-Mail veya şifre hatalı!");
            }


            private JwtSecurityToken GetToken(List<Claim> authClaims)
            {
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: Configuration["JWT:ValidIssuer"],
                    audience: Configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return token;
            }
        }
    }
}
