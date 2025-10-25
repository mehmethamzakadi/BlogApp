using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.BulkDelete;

public class BulkDeleteUsersCommand : IRequest<BulkDeleteUsersResponse>
{
    public List<int> UserIds { get; set; } = new();
}
