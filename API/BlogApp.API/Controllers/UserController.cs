using BlogApp.Application.DTOs.Common;
using BlogApp.Application.DTOs.Results;
using BlogApp.Application.Features.AppUsers.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class UserController : BaseApiController
    {
        [HttpGet]
        public async Task<BaseResult<IReadOnlyList<RsAppUser>>> Get()
        {
            return await Mediator.Send(new GetAllAppUsersQuery());
        }
    }
}