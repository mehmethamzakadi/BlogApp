using BlogApp.Application.Behaviors;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Create;

public sealed record CreatePostCommand(string Title, string Body, string Summary, string Thumbnail, bool IsPublished, Guid CategoryId) : IRequest<IResult>, IInvalidateCache
{
    /// <summary>
    /// Uses version-based cache invalidation strategy.
    /// Invalidating version keys automatically makes all related cached pages stale.
    /// </summary>
    public IEnumerable<string> GetCacheKeysToInvalidate()
    {
        // Invalidate version keys - this makes all cached pages stale automatically
        yield return CacheKeys.PostListVersion();
        yield return CacheKeys.PostsByCategoryVersion(CategoryId);
        yield return CacheKeys.CategoryGridVersion();
    }
}
