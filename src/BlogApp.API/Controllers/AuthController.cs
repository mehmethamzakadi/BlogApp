using System;
using System.Linq;
using BlogApp.Application.Features.Auths.Login;
using BlogApp.Application.Features.Auths.Logout;
using BlogApp.Application.Features.Auths.PasswordReset;
using BlogApp.Application.Features.Auths.PasswordVerify;
using BlogApp.Application.Features.Auths.RefreshToken;
using BlogApp.Application.Features.Auths.Register;
using BlogApp.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class AuthController(IMediator mediator) : BaseApiController(mediator)
    {
        private const string RefreshTokenCookieName = "blogapp_refresh_token";
        private const int RefreshTokenLifetimeDays = 7;

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

            SetRefreshTokenCookie(result.Data.RefreshToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            if (!TryGetRefreshTokenFromCookie(out var refreshToken))
            {
                ClearRefreshTokenCookie();
                return Unauthorized<LoginResponse>("Oturum doğrulanamadı.", "RefreshTokenMissing", default);
            }

            var result = await Mediator.Send(new RefreshTokenCommand(refreshToken));

            if (!result.Success || result.Data is null)
            {
                ClearRefreshTokenCookie();
                return Unauthorized(result);
            }

            SetRefreshTokenCookie(result.Data.RefreshToken);
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

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var options = CreateRefreshTokenCookieOptions();
            Response.Cookies.Append(RefreshTokenCookieName, refreshToken, options);
        }

        private void ClearRefreshTokenCookie()
        {
            var options = CreateRefreshTokenCookieOptions();
            options.Expires = DateTimeOffset.UtcNow.AddDays(-1);
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

        private CookieOptions CreateRefreshTokenCookieOptions()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = IsSecureRequest(),
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(RefreshTokenLifetimeDays)
            };
        }

        private bool IsSecureRequest()
        {
            if (Request.IsHttps)
            {
                return true;
            }

            if (Request.Headers.TryGetValue("X-Forwarded-Proto", out var protoValues))
            {
                return protoValues.Any(value => string.Equals(value, "https", StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }
    }
}
