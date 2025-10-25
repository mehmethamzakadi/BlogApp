using BlogApp.Application.Features.Auths.Login;
using BlogApp.Application.Features.Auths.PasswordReset;
using BlogApp.Application.Features.Auths.PasswordVerify;
using BlogApp.Application.Features.Auths.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class AuthController(IMediator mediator) : BaseApiController(mediator)
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await Mediator.Send(command);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var result = await Mediator.Send(command);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("password-reset")]
        public async Task<IActionResult> PasswordReset([FromBody] PasswordResetCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("password-verify")]
        public async Task<IActionResult> PasswordVerify([FromBody] PasswordVerifyCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }
    }
}
