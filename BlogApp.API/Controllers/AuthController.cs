using BlogApp.Application.Features.Authorizations.Commands.UserLogin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class AuthController : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginCommand userLogin)
        {
            var result = await Mediator.Send(userLogin);
            return result.Success ? Ok(result) : Unauthorized(result.Message);
        }
    }
}