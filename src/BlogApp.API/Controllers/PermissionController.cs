using BlogApp.Application.Features.Permissions.Queries.GetAll;
using BlogApp.Application.Features.Permissions.Queries.GetRolePermissions;
using BlogApp.Application.Features.Permissions.Commands.AssignPermissionsToRole;
using BlogApp.Domain.Constants;
using BlogApp.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers;

public class PermissionController(IMediator mediator) : BaseApiController(mediator)
{
    /// <summary>
    /// Tüm permission'ları getirir (gruplandırılmış)
    /// </summary>
    [HttpGet]
    [HasPermission(Permissions.RolesRead)]
    public async Task<IActionResult> GetAll()
    {
        var response = await Mediator.Send(new GetAllPermissionsQuery());
        return Ok(response);
    }

    /// <summary>
    /// Belirli bir role ait permission'ları getirir
    /// </summary>
    [HttpGet("role/{roleId}")]
    [HasPermission(Permissions.RolesRead)]
    public async Task<IActionResult> GetRolePermissions([FromRoute] int roleId)
    {
        var response = await Mediator.Send(new GetRolePermissionsQuery(roleId));
        return Ok(response);
    }

    /// <summary>
    /// Role permission atar (tüm permission'ları replace eder)
    /// </summary>
    [HttpPost("role/{roleId}")]
    [HasPermission(Permissions.RolesAssignPermissions)]
    public async Task<IActionResult> AssignPermissionsToRole([FromRoute] int roleId, [FromBody] AssignPermissionsToRoleCommand command)
    {
        if (roleId != command.RoleId)
            return BadRequest("ID mismatch");

        var response = await Mediator.Send(command);
        return Ok(response);
    }
}
