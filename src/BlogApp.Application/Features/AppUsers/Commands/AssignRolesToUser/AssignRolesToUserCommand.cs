using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.AssignRolesToUser;

public class AssignRolesToUserCommand : IRequest<IResult>
{
    public int UserId { get; set; }
    public List<int> RoleIds { get; set; } = new();
}
