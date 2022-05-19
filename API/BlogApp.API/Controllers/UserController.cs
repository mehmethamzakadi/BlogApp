using BlogApp.Application.DTOs.AppUsers;
using BlogApp.Application.Features.AppUsers.Commands;
using BlogApp.Application.Features.AppUsers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class UserController : BaseApiController
    {
        [HttpGet]
        public async Task<IReadOnlyList<AppUserResponseDto>> Get()
        {
            return await Mediator.Send(new GetAllUsersQuery());
        }

        [HttpGet("{id}")]
        public async Task<AppUserResponseDto> Get(int id)
        {
            return await Mediator.Send(new GetByIdUserQuery { Id = id });
        }

        [HttpPost]
        public async Task<bool> Post(CreateAppUserCommand user)
        {
            return await Mediator.Send(new CreateAppUserCommand { UserName = user.UserName, Email = user.Email, Password = user.Password });
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await Mediator.Send(new DeleteAppUserCommand { Id = id });
        }

        [HttpPut]
        public async Task Put(UpdateAppUserCommand user)
        {
            await Mediator.Send(new UpdateAppUserCommand { Id = user.Id, Email = user.Email, UserName = user.UserName });
        }
    }
}