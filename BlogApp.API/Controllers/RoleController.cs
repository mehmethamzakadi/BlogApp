using BlogApp.Application.Features.AppRoles.Commands.Create;
using BlogApp.Application.Features.AppRoles.Commands.Delete;
using BlogApp.Application.Features.AppRoles.Commands.Update;
using BlogApp.Application.Features.AppRoles.Queries.GetList;
using BlogApp.Application.Features.AppRoles.Queries.GetRoleById;
using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class RoleController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
        {
            GetListResponse<GetListAppRoleResponse> response = await Mediator.Send(new GetListRoleQuery(pageRequest));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetRoleByIdRequest(id));
            return Ok(response);
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand createRoleCommandRequest)
        {
            var addRole = await Mediator.Send(createRoleCommandRequest);
            return Ok(addRole);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleCommand updateRoleCommandRequest)
        {
            var updateRole = await Mediator.Send(updateRoleCommandRequest);
            return Ok(updateRole);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole([FromRoute] int id)
        {
            var deleteRole = await Mediator.Send(new DeleteRoleCommand(id));
            return Ok(deleteRole);
        }
    }
}