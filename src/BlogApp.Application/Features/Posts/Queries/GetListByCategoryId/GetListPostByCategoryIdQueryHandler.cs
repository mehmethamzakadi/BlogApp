using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Queries.GetListByCategoryId;

/// <summary>
/// Handler for getting paginated posts by category ID
/// ✅ PERFORMANCE: Using projection to avoid loading full entities
/// </summary>
public sealed class GetListPostByCategoryIdQueryHandler(IPostRepository postRepository)
    : IRequestHandler<GetListPostByCategoryIdQuery, PaginatedListResponse<GetListPostByCategoryIdResponse>>
{
    public async Task<PaginatedListResponse<GetListPostByCategoryIdResponse>> Handle(
        GetListPostByCategoryIdQuery request,
        CancellationToken cancellationToken
    )
    {
        // ✅ PERFORMANCE: Using projection to select only needed fields
        var paginated = await postRepository.GetPublishedPostsProjectedAsync(
            query => query.Select(p => new GetListPostByCategoryIdResponse(
                p.Id,
                p.Title,
                p.Body,
                p.Summary,
                p.Thumbnail,
                p.IsPublished,
                p.Category.Name,
                p.CategoryId,
                p.CreatedDate
            )),
            request.CategoryId,
            request.PageRequest.PageIndex,
            request.PageRequest.PageSize,
            cancellationToken
        );

        var response = new PaginatedListResponse<GetListPostByCategoryIdResponse>
        {
            Items = paginated.Items.ToList(),
            Index = paginated.Index,
            Size = paginated.Size,
            Count = paginated.Count,
            Pages = paginated.Pages
        };

        return response;
    }
}
