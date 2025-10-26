using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Users.Queries.GetUserRoles;

public record GetUserRolesQuery(Guid UserId) : IRequest<IDataResult<GetUserRolesResponse>>;
