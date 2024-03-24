using BlogApp.Application.Features.AppRoles.Commands.Create;
using BlogApp.Application.Features.AppRoles.Commands.Delete;
using BlogApp.Application.Features.AppRoles.Commands.Update;
using BlogApp.Application.Features.AppRoles.Queries.GetAllRoles;
using BlogApp.Application.Features.AppRoles.Queries.GetRoleById;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class RoleController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllRoles([FromQuery] GetAllRoleQueryRequest getAllRoleQueryRequest)
        {
            GetAllRoleQueryResponse response = await Mediator.Send(getAllRoleQueryRequest);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById([FromRoute] int id)
        {
            GetRoleByIdQueryResponse response = await Mediator.Send(new GetRoleByIdQueryRequest(id));
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