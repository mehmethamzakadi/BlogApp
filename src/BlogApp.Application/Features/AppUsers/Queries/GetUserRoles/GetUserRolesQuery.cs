using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Queries.GetUserRoles;

public record GetUserRolesQuery(int UserId) : IRequest<IDataResult<GetUserRolesResponse>>;
