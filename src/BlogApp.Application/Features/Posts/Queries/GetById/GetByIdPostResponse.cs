namespace BlogApp.Application.Features.Posts.Queries.GetById;

public sealed record GetByIdPostResponse(int Id, string Title, string Body, string Summary, string Thumbnail, bool IsPublished, string CategoryName, int CategoryId,DateTime CreatedDate);
