using MediatR;

namespace BlogApp.Application.Features.Users.Commands.BulkDelete;

public class BulkDeleteUsersCommand : IRequest<BulkDeleteUsersResponse>
{
    public List<Guid> UserIds { get; set; } = new();
}
