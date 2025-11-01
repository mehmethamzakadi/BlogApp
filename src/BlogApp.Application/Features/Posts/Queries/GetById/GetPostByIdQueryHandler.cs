using BlogApp.Application.Abstractions;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Application.Features.Posts.Queries.GetById;

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

        var response = await _postRepository.Query()
            .Where(b => b.Id == request.Id && (request.IncludeUnpublished || b.IsPublished))
            .Include(p => p.Category)
            .AsNoTracking()
            .Select(p => new GetByIdPostResponse(
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
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
            return new ErrorDataResult<GetByIdPostResponse>("Post bilgisi bulunamadı.");

        await _cacheService.Add(cacheKey, response, 
            absExpr: DateTimeOffset.UtcNow.Add(CacheDurations.Post),
            sldExpr: null);

        return new SuccessDataResult<GetByIdPostResponse>(response);
    }
}
