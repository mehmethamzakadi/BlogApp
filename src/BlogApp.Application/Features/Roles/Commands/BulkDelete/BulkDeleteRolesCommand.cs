using MediatR;

namespace BlogApp.Application.Features.Roles.Commands.BulkDelete;

public class BulkDeleteRolesCommand : IRequest<BulkDeleteRolesResponse>
{
    public List<Guid> RoleIds { get; set; } = new();
}
