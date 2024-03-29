using BlogApp.Application.Features.AppUsers.Commands.Login;
using BlogApp.Application.Features.AppUsers.Commands.PasswordReset;
using BlogApp.Application.Features.AppUsers.Commands.PasswordVerify;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class AuthController : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand userLogin)
        {
            var result = await Mediator.Send(userLogin);
            return result.Success ? Ok(result) : Unauthorized(result.Message);
        }

        [HttpPost("PasswordReset")]
        public async Task<IActionResult> PasswordReset(PasswordResetCommand passwordResetCommandRequest)
        {
            var response = await Mediator.Send(passwordResetCommandRequest);
            return Ok(response);
        }

        [HttpPost("PasswordVerify")]
        public async Task<IActionResult> PasswordVerify(PasswordVerifyCommand passwordVerifyCommandRequestCommandRequest)
        {
            var response = await Mediator.Send(passwordVerifyCommandRequestCommandRequest);
            return Ok(response);
        }
    }
}