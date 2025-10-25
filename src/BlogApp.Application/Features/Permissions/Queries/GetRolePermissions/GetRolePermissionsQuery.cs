using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Permissions.Queries.GetRolePermissions;

public record GetRolePermissionsQuery(int RoleId) : IRequest<IDataResult<GetRolePermissionsResponse>>;
