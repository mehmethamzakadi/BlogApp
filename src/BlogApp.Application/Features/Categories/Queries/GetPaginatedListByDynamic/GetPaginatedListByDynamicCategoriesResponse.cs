using BlogApp.Application.Common;

namespace BlogApp.Application.Features.Categories.Queries.GetPaginatedListByDynamic;

public sealed record GetPaginatedListByDynamicCategoriesResponse : BaseEntityResponse
{
    public string Name { get; init; } = string.Empty;
}
