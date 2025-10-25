namespace BlogApp.Application.Features.Posts.Queries.GetPaginatedListByDynamic;

public sealed record GetPaginatedListByDynamicPostsResponse(int Id, string Title, string Body, string Summary, string Thumbnail, bool IsPublished, string CategoryName, int CategoryId, DateTime CreatedDate);
