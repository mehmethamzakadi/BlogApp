using BlogApp.Application.Abstractions;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Queries.GetList;

/// <summary>
/// Handler for getting paginated list of published posts.
/// ✅ PERFORMANCE: Using projection to avoid loading full entities.
/// ✅ CACHING: Uses version-based cache invalidation strategy.
/// </summary>
public sealed class GetListPostQueryHandler(
    IPostRepository postRepository,
    ICacheService cacheService) : IRequestHandler<GetListPostQuery, PaginatedListResponse<GetListPostResponse>>
{
    public async Task<PaginatedListResponse<GetListPostResponse>> Handle(GetListPostQuery request, CancellationToken cancellationToken)
    {
        Guid? categoryId = (request.PageRequest as PostListRequest)?.CategoryId;
        var pageIndex = request.PageRequest.PageIndex;
        var pageSize = request.PageRequest.PageSize;

        // Get or create version token for cache key
        string versionKey;
        string cacheKey;
        
        if (categoryId.HasValue)
        {
            versionKey = CacheKeys.PostsByCategoryVersion(categoryId.Value);
            var versionToken = await GetOrCreateVersionToken(versionKey);
            cacheKey = CacheKeys.PostsByCategory(versionToken, categoryId.Value, pageIndex, pageSize);
        }
        else
        {
            versionKey = CacheKeys.PostListVersion();
            var versionToken = await GetOrCreateVersionToken(versionKey);
            cacheKey = CacheKeys.PostList(versionToken, pageIndex, pageSize);
        }

        // Try to get from cache
        var cachedResponse = await cacheService.Get<PaginatedListResponse<GetListPostResponse>>(cacheKey);
        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

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
            pageIndex,
            pageSize,
            cancellationToken);

        var response = new PaginatedListResponse<GetListPostResponse>
        {
            Items = [.. paginated.Items],
            Index = paginated.Index,
            Size = paginated.Size,
            Count = paginated.Count,
            Pages = paginated.Pages
        };

        // Cache the response
        await cacheService.Add(
            cacheKey,
            response,
            DateTimeOffset.UtcNow.Add(CacheDurations.Post),
            null);

        return response;
    }

    private async Task<string> GetOrCreateVersionToken(string versionKey)
    {
        var versionToken = await cacheService.Get<string>(versionKey);
        if (string.IsNullOrWhiteSpace(versionToken))
        {
            versionToken = Guid.NewGuid().ToString("N");
            await cacheService.Add(versionKey, versionToken, null, null);
        }
        return versionToken;
    }
}
