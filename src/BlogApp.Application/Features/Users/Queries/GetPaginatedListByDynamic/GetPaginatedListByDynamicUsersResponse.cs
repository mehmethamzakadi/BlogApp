using BlogApp.Application.Common;

namespace BlogApp.Application.Features.Users.Queries.GetPaginatedListByDynamic;

public sealed record GetPaginatedListByDynamicUsersResponse : BaseEntityResponse
{
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
