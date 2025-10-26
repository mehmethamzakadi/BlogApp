using BlogApp.Application.Common;

namespace BlogApp.Application.Features.Posts.Queries.GetPaginatedListByDynamic;

public sealed record GetPaginatedListByDynamicPostsResponse : BaseEntityResponse
{
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string Thumbnail { get; init; } = string.Empty;
    public bool IsPublished { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
}
