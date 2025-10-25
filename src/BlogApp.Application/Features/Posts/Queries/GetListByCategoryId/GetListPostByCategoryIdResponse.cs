namespace BlogApp.Application.Features.Posts.Queries.GetListByCategoryId;

public sealed record GetListPostByCategoryIdResponse(
    int Id,
    string Title,
    string Body,
    string Summary,
    string Thumbnail,
    bool IsPublished,
    string CategoryName,
    int CategoryId,
    DateTime CreatedDate
);
