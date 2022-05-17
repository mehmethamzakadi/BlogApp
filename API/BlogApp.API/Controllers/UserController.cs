using BlogApp.Application.DTOs;
using BlogApp.Application.DTOs.Common;
using BlogApp.Application.Features.AppUsers.Commands;
using BlogApp.Application.Features.AppUsers.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class UserController : BaseApiController
    {
        [HttpGet]
        public async Task<BaseResult<IReadOnlyList<AppUserDto>>> Get()
        {
            return await Mediator.Send(new GetAllUsersQuery());
        }

        [HttpGet("{id}")]
        public async Task<BaseResult<AppUserDto>> Get(int id)
        {
            return await Mediator.Send(new GetByIdUserQuery { Id = id });
        }

        [HttpPost]
        public async Task<BaseResult<CreateAppUserCommand>> Post(AppUserCreateDto user)
        {
            return await Mediator.Send(new CreateAppUserCommand { User = user });
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await Mediator.Send(new DeleteAppUserCommand { Id = id });
        }

        [HttpPut]
        public async Task Put(AppUserUpdateDto user)
        {
            await Mediator.Send(new UpdateAppUserCommand { User = user });
        }
    }
}