using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Users.Commands.AssignRolesToUser;

public class AssignRolesToUserCommand : IRequest<IResult>
{
    public Guid UserId { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
}
