using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Queries.GetList;

/// <summary>
/// Handler for getting paginated list of published posts
/// ✅ PERFORMANCE: Using projection to avoid loading full entities
/// </summary>
public sealed class GetListPostQueryHandler(IPostRepository postRepository) : IRequestHandler<GetListPostQuery, PaginatedListResponse<GetListPostResponse>>
{
    public async Task<PaginatedListResponse<GetListPostResponse>> Handle(GetListPostQuery request, CancellationToken cancellationToken)
    {
        Guid? categoryId = (request.PageRequest as PostListRequest)?.CategoryId;

        // ✅ PERFORMANCE: Using projection to select only needed fields
        var paginated = await postRepository.GetPublishedPostsProjectedAsync(
            query => query.Select(p => new GetListPostResponse(
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
            categoryId,
            request.PageRequest.PageIndex,
            request.PageRequest.PageSize,
            cancellationToken);

        var response = new PaginatedListResponse<GetListPostResponse>
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
