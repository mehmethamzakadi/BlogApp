using BlogApp.Application.Features.AppUsers.Commands.Create;
using BlogApp.Application.Features.AppUsers.Commands.Delete;
using BlogApp.Application.Features.AppUsers.Commands.Update;
using BlogApp.Application.Features.AppUsers.Queries.GetById;
using BlogApp.Application.Features.AppUsers.Queries.GetList;
using BlogApp.Application.Utilities.Requests;
using BlogApp.Application.Utilities.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class UserController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
        {
            GetListAppUsersQuery getListAppUserQuery = new() { PageRequest = pageRequest };
            GetListResponse<GetListAppUserResponse> response = await Mediator.Send(getListAppUserQuery);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return GetResponseOnlyResultData(await Mediator.Send(new GetByIdAppUserQuery { Id = id }));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post(CreateAppUserCommand user)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(user));
        }

        [HttpPut]
        public async Task<IActionResult> Put(UpdateAppUserCommand updateUser)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(updateUser));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteAppUserCommand deleteUser)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(deleteUser));
        }
    }
}