namespace BlogApp.Application.Features.Posts.Queries.GetList;

public sealed record GetListPostResponse(int Id, string Title, string Body, string Summary, string Thumbnail, bool IsPublished, string CategoryName, int CategoryId, DateTime CreatedDate);
