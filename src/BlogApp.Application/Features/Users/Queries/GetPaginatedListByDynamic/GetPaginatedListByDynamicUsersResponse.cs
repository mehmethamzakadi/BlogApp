using System.Collections.Generic;
using BlogApp.Application.Common;

namespace BlogApp.Application.Features.Users.Queries.GetPaginatedListByDynamic;

public sealed record GetPaginatedListByDynamicUsersResponse : BaseEntityResponse
{
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public IReadOnlyCollection<GetPaginatedListByDynamicUserRoleResponse> Roles { get; init; } = Array.Empty<GetPaginatedListByDynamicUserRoleResponse>();
}

public sealed record GetPaginatedListByDynamicUserRoleResponse(Guid Id, string Name);
