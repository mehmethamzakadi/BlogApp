using BlogApp.Application.Features.Auths.Login;
using BlogApp.Application.Features.Auths.Logout;
using BlogApp.Application.Features.Auths.PasswordReset;
using BlogApp.Application.Features.Auths.PasswordVerify;
using BlogApp.Application.Features.Auths.RefreshToken;
using BlogApp.Application.Features.Auths.Register;
using BlogApp.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class AuthController(IMediator mediator) : BaseApiController(mediator)
    {
        private const string RefreshTokenCookieName = "blogapp_refresh_token";

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await Mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await Mediator.Send(command);

            if (!result.Success || result.Data is null)
            {
                ClearRefreshTokenCookie();
                return Unauthorized(result);
            }

            SetRefreshTokenCookie(result.Data);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            if (!TryGetRefreshTokenFromCookie(out var refreshToken))
            {
                ClearRefreshTokenCookie();
                return Unauthorized();
            }

            var result = await Mediator.Send(new RefreshTokenCommand(refreshToken));

            if (!result.Success || result.Data is null)
            {
                ClearRefreshTokenCookie();
                return Unauthorized(result);
            }

            SetRefreshTokenCookie(result.Data);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (!TryGetRefreshTokenFromCookie(out var refreshToken))
            {
                ClearRefreshTokenCookie();
                return Ok(new ApiResult<object>
                {
                    Success = true,
                    Message = "Çıkış yapıldı.",
                    InternalMessage = "LogoutWithoutRefreshToken",
                    Data = null
                });
            }

            await Mediator.Send(new LogoutCommand(refreshToken));
            ClearRefreshTokenCookie();

            return Ok(new ApiResult<object>
            {
                Success = true,
                Message = "Çıkış yapıldı.",
                InternalMessage = "Logout",
                Data = null
            });
        }

        [AllowAnonymous]
        [HttpPost("password-reset")]
        public async Task<IActionResult> PasswordReset([FromBody] PasswordResetCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("password-verify")]
        public async Task<IActionResult> PasswordVerify([FromBody] PasswordVerifyCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        private void SetRefreshTokenCookie(LoginResponse response)
        {
            var options = CreateRefreshTokenCookieOptions(response.RefreshTokenExpiration);
            Response.Cookies.Append(RefreshTokenCookieName, response.RefreshToken, options);
        }

        private void ClearRefreshTokenCookie()
        {
            var options = CreateRefreshTokenCookieOptions(DateTime.UtcNow.AddYears(-1));
            Response.Cookies.Append(RefreshTokenCookieName, string.Empty, options);
        }

        private bool TryGetRefreshTokenFromCookie(out string refreshToken)
        {
            if (Request.Cookies.TryGetValue(RefreshTokenCookieName, out var token) && !string.IsNullOrWhiteSpace(token))
            {
                refreshToken = token;
                return true;
            }

            refreshToken = string.Empty;
            return false;
        }

        private CookieOptions CreateRefreshTokenCookieOptions(DateTime refreshExpirationUtc)
        {
            var environment = HttpContext?.RequestServices.GetService<IWebHostEnvironment>();
            var isDevelopment = environment?.IsDevelopment() ?? false;

            var host = Request.Host.Host;
            string? cookieDomain = null;

            if (!string.IsNullOrWhiteSpace(host) && host.Contains('.'))
            {
                cookieDomain = host;
            }

            var requestIsHttps = Request.IsHttps;

            var sameSite = SameSiteMode.Strict;
            var secure = requestIsHttps;

            if (Request.Headers.TryGetValue("Origin", out var originHeader) &&
                Uri.TryCreate(originHeader.ToString(), UriKind.Absolute, out var originUri) &&
                !string.Equals(originUri.Host, host, StringComparison.OrdinalIgnoreCase))
            {
                if (requestIsHttps)
                {
                    sameSite = SameSiteMode.None;
                    secure = true;
                }
                else
                {
                    sameSite = SameSiteMode.Lax;
                }
            }
            else if (isDevelopment && !requestIsHttps)
            {
                sameSite = SameSiteMode.Lax;
            }

            if (!requestIsHttps && !isDevelopment)
            {
                secure = false;
            }

            return new CookieOptions
            {
                HttpOnly = true,
                Secure = secure,
                SameSite = sameSite,
                Path = "/",
                Expires = new DateTimeOffset(DateTime.SpecifyKind(refreshExpirationUtc, DateTimeKind.Utc)),
                Domain = cookieDomain
            };
        }

    }
}
