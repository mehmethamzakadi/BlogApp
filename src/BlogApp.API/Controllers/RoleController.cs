using BlogApp.Application.Features.Roles.Commands.BulkDelete;
using BlogApp.Application.Features.Roles.Commands.Create;
using BlogApp.Application.Features.Roles.Commands.Delete;
using BlogApp.Application.Features.Roles.Commands.Update;
using BlogApp.Application.Features.Roles.Queries.GetList;
using BlogApp.Application.Features.Roles.Queries.GetRoleById;
using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Constants;
using BlogApp.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class RoleController(IMediator mediator) : BaseApiController(mediator)
    {
        [HttpGet]
        [HasPermission(Permissions.RolesViewAll)]
        public async Task<IActionResult> GetList([FromQuery] PaginatedRequest pageRequest)
        {
            PaginatedListResponse<GetListRoleResponse> response = await Mediator.Send(new GetListRoleQuery(pageRequest));
            return Ok(response);
        }

        [HttpGet("{id}")]
        [HasPermission(Permissions.RolesRead)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetRoleByIdRequest(id));
            return Ok(response);
        }

        [HttpPost]
        [HasPermission(Permissions.RolesCreate)]
        public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("{id}")]
        [HasPermission(Permissions.RolesUpdate)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateRoleCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            var response = await Mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [HasPermission(Permissions.RolesDelete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var response = await Mediator.Send(new DeleteRoleCommand(id));
            return Ok(response);
        }

        /// <summary>
        /// Birden fazla rolü toplu olarak siler
        /// </summary>
        [HttpPost("bulk-delete")]
        [HasPermission(Permissions.RolesDelete)]
        public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteRolesCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }
    }
}
