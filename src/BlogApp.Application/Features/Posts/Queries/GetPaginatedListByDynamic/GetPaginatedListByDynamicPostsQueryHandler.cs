using AutoMapper;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Application.Features.Posts.Queries.GetPaginatedListByDynamic;

public sealed class GetPaginatedListByDynamicPostsQueryHandler(
    IPostRepository postRepository,
    IMapper mapper) : IRequestHandler<GetPaginatedListByDynamicPostsQuery, PaginatedListResponse<GetPaginatedListByDynamicPostsResponse>>
{
    public async Task<PaginatedListResponse<GetPaginatedListByDynamicPostsResponse>> Handle(GetPaginatedListByDynamicPostsQuery request, CancellationToken cancellationToken)
    {
        Paginate<Post> postsDynamic = await postRepository.GetPaginatedListByDynamicAsync(
        dynamic: request.DataGridRequest.DynamicQuery,
        index: request.DataGridRequest.PaginatedRequest.PageIndex,
        size: request.DataGridRequest.PaginatedRequest.PageSize,
        include: q => q.Include(x => x.Category),
        cancellationToken: cancellationToken
        );

        PaginatedListResponse<GetPaginatedListByDynamicPostsResponse> response = mapper.Map<PaginatedListResponse<GetPaginatedListByDynamicPostsResponse>>(postsDynamic);
        return response;
    }
}
