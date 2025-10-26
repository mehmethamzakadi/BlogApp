using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Permissions.Queries.GetRolePermissions;

public record GetRolePermissionsQuery(Guid RoleId) : IRequest<IDataResult<GetRolePermissionsResponse>>;
