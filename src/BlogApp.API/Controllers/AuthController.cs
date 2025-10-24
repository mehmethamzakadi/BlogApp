using BlogApp.Application.Features.Auths.Login;
using BlogApp.Application.Features.Auths.PasswordReset;
using BlogApp.Application.Features.Auths.PasswordVerify;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class AuthController(IMediator mediator) : BaseApiController(mediator)
    {
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand userLogin)
        {
            var result = await Mediator.Send(userLogin);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("PasswordReset")]
        public async Task<IActionResult> PasswordReset([FromBody] PasswordResetCommand passwordResetCommandRequest)
        {
            var response = await Mediator.Send(passwordResetCommandRequest);
            return Ok(response);
        }

        [HttpPost("PasswordVerify")]
        public async Task<IActionResult> PasswordVerify([FromBody] PasswordVerifyCommand passwordVerifyCommandRequest)
        {
            var response = await Mediator.Send(passwordVerifyCommandRequest);
            return Ok(response);
        }
    }
}
