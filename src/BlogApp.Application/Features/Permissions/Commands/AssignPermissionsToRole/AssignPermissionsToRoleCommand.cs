using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Permissions.Commands.AssignPermissionsToRole;

public class AssignPermissionsToRoleCommand : IRequest<IResult>
{
    public int RoleId { get; set; }
    public List<int> PermissionIds { get; set; } = new();
}
