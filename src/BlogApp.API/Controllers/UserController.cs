using BlogApp.Application.Features.Auths.UpdatePassword;
using BlogApp.Application.Features.Users.Commands.AssignRolesToUser;
using BlogApp.Application.Features.Users.Commands.BulkDelete;
using BlogApp.Application.Features.Users.Commands.Create;
using BlogApp.Application.Features.Users.Commands.Delete;
using BlogApp.Application.Features.Users.Commands.Update;
using BlogApp.Application.Features.Users.Queries.Export;
using BlogApp.Application.Features.Users.Queries.GetById;
using BlogApp.Application.Features.Users.Queries.GetPaginatedListByDynamic;
using BlogApp.Application.Features.Users.Queries.GetUserRoles;
using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Constants;
using BlogApp.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class UserController(IMediator mediator) : BaseApiController(mediator)
    {
        [HttpPost("search")]
        [HasPermission(Permissions.UsersViewAll)]
        public async Task<IActionResult> Search([FromBody] DataGridRequest dataGridRequest)
        {
            PaginatedListResponse<GetPaginatedListByDynamicUsersResponse> response = await Mediator.Send(new GetPaginatedListByDynamicUsersQuery(dataGridRequest));
            return Ok(response);
        }

        [HttpGet("{id}")]
        [HasPermission(Permissions.UsersRead)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetByIdUserQuery(id));
            return Ok(response);
        }

        [HttpPost]
        [HasPermission(Permissions.UsersCreate)]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("{id}")]
        [HasPermission(Permissions.UsersUpdate)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            var response = await Mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [HasPermission(Permissions.UsersDelete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var response = await Mediator.Send(new DeleteUserCommand(id));
            return Ok(response);
        }

        [HttpPost("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommand command)
        {
            UpdatePasswordResponse response = await Mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Kullanıcının rollerini getirir
        /// </summary>
        [HttpGet("{id}/roles")]
        [HasPermission(Permissions.RolesRead)]
        public async Task<IActionResult> GetUserRoles([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetUserRolesQuery(id));
            return Ok(response);
        }

        /// <summary>
        /// Kullanıcıya rol atar (tüm rolleri replace eder)
        /// </summary>
        [HttpPost("{id}/roles")]
        [HasPermission(Permissions.RolesAssignPermissions)]
        public async Task<IActionResult> AssignRolesToUser([FromRoute] int id, [FromBody] AssignRolesToUserCommand command)
        {
            if (id != command.UserId)
                return BadRequest("ID mismatch");

            var response = await Mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Birden fazla kullanıcıyı toplu olarak siler
        /// </summary>
        [HttpPost("bulk-delete")]
        [HasPermission(Permissions.UsersDelete)]
        public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteUsersCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Kullanıcıları CSV formatında export eder
        /// </summary>
        [HttpGet("export")]
        [HasPermission(Permissions.UsersViewAll)]
        public async Task<IActionResult> Export([FromQuery] string format = "csv")
        {
            var response = await Mediator.Send(new ExportUsersQuery { Format = format });
            return File(response.FileContent, response.ContentType, response.FileName);
        }
    }
}
