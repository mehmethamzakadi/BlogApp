using BlogApp.Application.Features.AppUsers.Commands;
using BlogApp.Application.Features.AppUsers.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class UserController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return GetResponseOnlyResultData(await Mediator.Send(new GetAllUsersQuery()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return GetResponseOnlyResultData(await Mediator.Send(new GetByIdUserQuery { Id = id }));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateAppUserCommand user)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(user));
        }

        [HttpPut]
        public async Task<IActionResult> Put(UpdateAppUserCommand user)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(user));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(new DeleteAppUserCommand { Id = id }));
        }
    }
}