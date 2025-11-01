using BlogApp.Application.Abstractions;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Application.Features.Posts.Queries.GetById;

/// <summary>
/// Handler for getting a post by ID
/// ✅ PERFORMANCE: Using projection to select only required fields
/// </summary>
public sealed class GetPostByIdQueryHandler : IRequestHandler<GetByIdPostQuery, IDataResult<GetByIdPostResponse>>
{
    private readonly IPostRepository _postRepository;
    private readonly ICacheService _cacheService;

    public GetPostByIdQueryHandler(IPostRepository postRepository, ICacheService cacheService)
    {
        _postRepository = postRepository;
        _cacheService = cacheService;
    }

    public async Task<IDataResult<GetByIdPostResponse>> Handle(GetByIdPostQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = request.IncludeUnpublished 
            ? CacheKeys.PostWithDrafts(request.Id) 
            : CacheKeys.PostPublic(request.Id);

        var cached = await _cacheService.Get<GetByIdPostResponse>(cacheKey);
        if (cached != null)
        {
            return new SuccessDataResult<GetByIdPostResponse>(cached);
        }

        // ✅ PERFORMANCE: Using projection instead of loading full entity
        // Select only the fields we need instead of entire Post + Category
        var response = await _postRepository.GetByIdProjectedAsync(
            request.Id,
            request.IncludeUnpublished,
            query => query.Select(p => new GetByIdPostResponse(
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
            cancellationToken);

        if (response is null)
            return new ErrorDataResult<GetByIdPostResponse>("Post bilgisi bulunamadı.");

        await _cacheService.Add(cacheKey, response, 
            absExpr: DateTimeOffset.UtcNow.Add(CacheDurations.Post),
            sldExpr: null);

        return new SuccessDataResult<GetByIdPostResponse>(response);
    }
}
