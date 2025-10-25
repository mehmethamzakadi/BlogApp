using MediatR;

namespace BlogApp.Application.Features.AppRoles.Commands.BulkDelete;

public class BulkDeleteRolesCommand : IRequest<BulkDeleteRolesResponse>
{
    public List<int> RoleIds { get; set; } = new();
}
