using BlogApp.Application.Features.AppUsers.Commands.Create;
using BlogApp.Application.Features.AppUsers.Commands.Delete;
using BlogApp.Application.Features.AppUsers.Commands.Update;
using BlogApp.Application.Features.AppUsers.Commands.UpdatePassword;
using BlogApp.Application.Features.AppUsers.Queries.GetById;
using BlogApp.Application.Features.AppUsers.Queries.GetList;
using BlogApp.Domain.Common.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class UserController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
        {
            return GetResponse(await Mediator.Send(new GetListAppUsersQuery(pageRequest)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            return GetResponse(await Mediator.Send(new GetByIdAppUserQuery(id)));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateAppUserCommand user)
        {
            return GetResponse(await Mediator.Send(user));
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateAppUserCommand updateUser)
        {
            return GetResponse(await Mediator.Send(updateUser));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteAppUserCommand deleteUser)
        {
            return GetResponse(await Mediator.Send(deleteUser));
        }

        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommand updatePasswordCommandRequest)
        {
            return GetResponse(await Mediator.Send(updatePasswordCommandRequest));
        }
    }
}