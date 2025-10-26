using AutoMapper;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlogApp.Application.Features.Posts.Queries.GetList;

public sealed class GetListPostQueryHandler(IPostRepository postRepository, IMapper mapper) : IRequestHandler<GetListPostQuery, PaginatedListResponse<GetListPostResponse>>
{
    public async Task<PaginatedListResponse<GetListPostResponse>> Handle(GetListPostQuery request, CancellationToken cancellationToken)
    {
        Guid? categoryId = (request.PageRequest as PostListRequest)?.CategoryId;

        Expression<Func<Post, bool>> predicate = categoryId.HasValue && categoryId != Guid.Empty
            ? post => post.IsPublished && post.CategoryId == categoryId.Value
            : post => post.IsPublished;

        Paginate<Post> posts = await postRepository.GetPaginatedListAsync(
            predicate: predicate,
            orderBy: query => query.OrderByDescending(post => post.Id),
            index: request.PageRequest.PageIndex,
            include: p => p.Include(p => p.Category),
            size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken
        );

        PaginatedListResponse<GetListPostResponse> response = mapper.Map<PaginatedListResponse<GetListPostResponse>>(posts);
        return response;
    }
}
