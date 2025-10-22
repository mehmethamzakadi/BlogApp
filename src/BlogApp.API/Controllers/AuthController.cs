using BlogApp.Application.Features.Auths.Login;
using BlogApp.Application.Features.Auths.PasswordReset;
using BlogApp.Application.Features.Auths.PasswordVerify;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    [AllowAnonymous]
    public class AuthController(IMediator mediator) : BaseApiController(mediator)
    {
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginCommand userLogin)
        {
            var result = await Mediator.Send(userLogin);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [HttpPost]
        public async Task<IActionResult> PasswordReset([FromBody] PasswordResetCommand passwordResetCommandRequest)
        {
            var response = await Mediator.Send(passwordResetCommandRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> PasswordVerify([FromBody] PasswordVerifyCommand passwordVerifyCommandRequest)
        {
            var response = await Mediator.Send(passwordVerifyCommandRequest);
            return Ok(response);
        }
    }
}
