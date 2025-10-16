using BlogApp.Application.Features.AppUsers.Commands.Create;
using BlogApp.Application.Features.AppUsers.Commands.Delete;
using BlogApp.Application.Features.AppUsers.Commands.Update;
using BlogApp.Application.Features.AppUsers.Queries.GetById;
using BlogApp.Application.Features.AppUsers.Queries.GetList;
using BlogApp.Application.Features.AppUsers.Queries.GetPaginatedListByDynamic;
using BlogApp.Application.Features.Auths.UpdatePassword;
using BlogApp.Application.Features.Categories.Queries.GetPaginatedListByDynamic;
using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class UserController(IMediator mediator) : BaseApiController(mediator)
    {
        [HttpPost("GetPaginatedList")]
        public async Task<IActionResult> GetPaginatedListByDynamic(DataGridRequest dataGridRequest)
        {
            PaginatedListResponse<GetPaginatedListByDynamicUsersResponse> response = await Mediator.Send(new GetPaginatedListByDynamicUsersQuery(dataGridRequest));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await Mediator.Send(new GetByIdAppUserQuery(id));
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Post(CreateAppUserCommand user)
        {
            var response = await Mediator.Send(user);
            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Put(UpdateAppUserCommand updateUser)
        {
            var response = await Mediator.Send(updateUser);
            return Ok(response);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await Mediator.Send(new DeleteAppUserCommand(id));
            return Ok(response);
        }

        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommand updatePasswordCommandRequest)
        {
            UpdatePasswordResponse response = await Mediator.Send(updatePasswordCommandRequest);
            return Ok(response);
        }
    }
}