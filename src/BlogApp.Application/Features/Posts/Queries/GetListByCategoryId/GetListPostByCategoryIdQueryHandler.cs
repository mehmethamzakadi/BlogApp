using System.Linq;
using AutoMapper;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Application.Features.Posts.Queries.GetListByCategoryId;

public sealed class GetListPostByCategoryIdQueryHandler(IPostRepository postRepository, IMapper mapper)
    : IRequestHandler<GetListPostByCategoryIdQuery, PaginatedListResponse<GetListPostByCategoryIdResponse>>
{
    public async Task<PaginatedListResponse<GetListPostByCategoryIdResponse>> Handle(
        GetListPostByCategoryIdQuery request,
        CancellationToken cancellationToken
    )
    {
        Paginate<Post> posts = await postRepository.GetPaginatedListAsync(
            predicate: post => post.IsPublished && post.CategoryId == request.CategoryId,
            orderBy: query => query.OrderByDescending(post => post.Id),
            include: p => p.Include(p => p.Category),
            index: request.PageRequest.PageIndex,
            size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken
        );

        PaginatedListResponse<GetListPostByCategoryIdResponse> response =
            mapper.Map<PaginatedListResponse<GetListPostByCategoryIdResponse>>(posts);

        return response;
    }
}
