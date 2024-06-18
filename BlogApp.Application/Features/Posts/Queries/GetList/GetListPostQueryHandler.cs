using AutoMapper;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Application.Features.Posts.Queries.GetList;

public sealed class GetListPostQueryHandler(IPostRepository postRepository, IMapper mapper) : IRequestHandler<GetListPostQuery, Result<GetListResponse<GetListPostResponse>>>
{
    public async Task<Result<GetListResponse<GetListPostResponse>>> Handle(GetListPostQuery request, CancellationToken cancellationToken)
    {
        Paginate<Post> posts = await postRepository.GetListAsync(
            index: request.PageRequest.PageIndex,
            include: p => p.Include(p => p.Category),
            size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken
       );

        GetListResponse<GetListPostResponse> response = mapper.Map<GetListResponse<GetListPostResponse>>(posts);
        return Result<GetListResponse<GetListPostResponse>>.SuccessResult(response);
    }
}
