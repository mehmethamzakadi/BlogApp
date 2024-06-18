using BlogApp.Application.Features.AppRoles.Commands.Create;
using BlogApp.Application.Features.AppRoles.Commands.Delete;
using BlogApp.Application.Features.AppRoles.Commands.Update;
using BlogApp.Application.Features.AppRoles.Queries.GetList;
using BlogApp.Application.Features.AppRoles.Queries.GetRoleById;
using BlogApp.Domain.Common.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class RoleController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
        {
            return GetResponse(await Mediator.Send(new GetListRoleQuery(pageRequest)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById([FromRoute] int id)
        {
            return GetResponse(await Mediator.Send(new GetRoleByIdRequest(id)));
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand createRoleCommandRequest)
        {
            return GetResponse(await Mediator.Send(createRoleCommandRequest));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleCommand updateRoleCommandRequest)
        {
            return GetResponse(await Mediator.Send(updateRoleCommandRequest));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole([FromRoute] int id)
        {
            return GetResponse(await Mediator.Send(new DeleteRoleCommand(id)));
        }
    }
}