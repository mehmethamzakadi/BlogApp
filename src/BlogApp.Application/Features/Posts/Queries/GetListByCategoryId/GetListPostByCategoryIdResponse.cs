namespace BlogApp.Application.Features.Posts.Queries.GetListByCategoryId;

public sealed record GetListPostByCategoryIdResponse(
    Guid Id,
    string Title,
    string Body,
    string Summary,
    string Thumbnail,
    bool IsPublished,
    string CategoryName,
    Guid CategoryId,
    DateTime CreatedDate
);
