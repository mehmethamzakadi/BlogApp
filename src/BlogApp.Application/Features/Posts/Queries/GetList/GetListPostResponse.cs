namespace BlogApp.Application.Features.Posts.Queries.GetList;

public sealed record GetListPostResponse(Guid Id, string Title, string Body, string Summary, string Thumbnail, bool IsPublished, string CategoryName, Guid CategoryId, DateTime CreatedDate);
