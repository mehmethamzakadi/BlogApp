using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Permissions.Commands.AssignPermissionsToRole;

public class AssignPermissionsToRoleCommand : IRequest<IResult>
{
    public Guid RoleId { get; set; }
    public List<Guid> PermissionIds { get; set; } = new();
}
