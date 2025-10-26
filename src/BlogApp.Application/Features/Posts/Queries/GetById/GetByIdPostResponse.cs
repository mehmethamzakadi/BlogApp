namespace BlogApp.Application.Features.Posts.Queries.GetById;

public sealed record GetByIdPostResponse(Guid Id, string Title, string Body, string Summary, string Thumbnail, bool IsPublished, string CategoryName, Guid CategoryId, DateTime CreatedDate);
