using BlogApp.Application.Features.AppUsers.Commands.Create;
using BlogApp.Application.Features.AppUsers.Commands.Delete;
using BlogApp.Application.Features.AppUsers.Commands.Update;
using BlogApp.Application.Features.AppUsers.Queries.GetById;
using BlogApp.Application.Features.AppUsers.Queries.GetList;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class UserController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return GetResponseOnlyResultData(await Mediator.Send(new GetListAppUsersQuery()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return GetResponseOnlyResultData(await Mediator.Send(new GetByIdAppUserQuery { Id = id }));
        }

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