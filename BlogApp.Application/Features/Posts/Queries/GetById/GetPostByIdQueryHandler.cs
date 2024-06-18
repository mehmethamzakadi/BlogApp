using AutoMapper;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Queries.GetById;

public sealed class GetPostByIdQueryHandler(IPostRepository postRepository, IMapper mapper) : IRequestHandler<GetByIdPostQuery, Result<GetByIdPostResponse>>
{
    public async Task<Result<GetByIdPostResponse>> Handle(GetByIdPostQuery request, CancellationToken cancellationToken)
    {
        Post? post = await postRepository.GetAsync(predicate: b => b.Id == request.Id, cancellationToken: cancellationToken);
        if (post is null)
            return Result<GetByIdPostResponse>.FailureResult("Post bilgisi bulunamadı.");

        GetByIdPostResponse response = mapper.Map<GetByIdPostResponse>(post);

        return Result<GetByIdPostResponse>.SuccessResult(response);
    }
}
