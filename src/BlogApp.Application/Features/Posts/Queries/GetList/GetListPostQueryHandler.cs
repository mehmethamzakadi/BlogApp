using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Application.Features.Posts.Queries.GetList;

public sealed class GetListPostQueryHandler(IPostRepository postRepository) : IRequestHandler<GetListPostQuery, PaginatedListResponse<GetListPostResponse>>
{
    public async Task<PaginatedListResponse<GetListPostResponse>> Handle(GetListPostQuery request, CancellationToken cancellationToken)
    {
        Guid? categoryId = (request.PageRequest as PostListRequest)?.CategoryId;

        var query = postRepository.Query()
            .Where(post => post.IsPublished)
            .Include(p => p.Category)
            .AsNoTracking();

        if (categoryId.HasValue && categoryId != Guid.Empty)
        {
            query = query.Where(post => post.CategoryId == categoryId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(post => post.CreatedDate)
            .Skip(request.PageRequest.PageIndex * request.PageRequest.PageSize)
            .Take(request.PageRequest.PageSize)
            .Select(p => new GetListPostResponse(
                p.Id,
                p.Title,
                p.Body,
                p.Summary,
                p.Thumbnail,
                p.IsPublished,
                p.Category.Name,
                p.CategoryId,
                p.CreatedDate
            ))
            .ToListAsync(cancellationToken);

        var response = new PaginatedListResponse<GetListPostResponse>
        {
            Items = items
        };
        response.Index = request.PageRequest.PageIndex;
        response.Size = request.PageRequest.PageSize;
        response.Count = totalCount;
        response.Pages = (int)Math.Ceiling(totalCount / (double)request.PageRequest.PageSize);

        return response;
    }
}
