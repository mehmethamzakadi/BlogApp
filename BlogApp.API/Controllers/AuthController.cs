using BlogApp.Application.Features.Authorizations.Queries.UserLogin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class AuthController : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginQuery userLogin)
        {
            var result = await Mediator.Send(userLogin);
            return result.Success ? Ok(result) : Unauthorized(result.Message);
        }
    }
}