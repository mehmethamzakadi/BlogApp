using AutoMapper;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Application.Features.Posts.Queries.GetList;

public sealed class GetListPostQueryHandler(IPostRepository postRepository, IMapper mapper) : IRequestHandler<GetListPostQuery, PaginatedListResponse<GetListPostResponse>>
{
    public async Task<PaginatedListResponse<GetListPostResponse>> Handle(GetListPostQuery request, CancellationToken cancellationToken)
    {
        Paginate<Post> posts = await postRepository.GetPaginatedListAsync(
            index: request.PageRequest.PageIndex,
            include: p => p.Include(p => p.Category),
            size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken
       );

        PaginatedListResponse<GetListPostResponse> response = mapper.Map<PaginatedListResponse<GetListPostResponse>>(posts);
        return response;
    }
}
