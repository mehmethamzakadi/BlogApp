using BlogApp.Application.Behaviors;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Delete;

public sealed record DeletePostCommand(Guid Id) : IRequest<IResult>, IInvalidateCache
{
    /// <summary>
    /// Uses version-based cache invalidation strategy.
    /// Invalidating version keys automatically makes all related cached pages stale.
    /// </summary>
    public IEnumerable<string> GetCacheKeysToInvalidate()
    {
        // Invalidate single post caches
        yield return CacheKeys.Post(Id);
        yield return CacheKeys.PostPublic(Id);
        yield return CacheKeys.PostWithDrafts(Id);
        
        // Invalidate version keys - this makes all cached pages stale automatically
        yield return CacheKeys.PostListVersion();
        yield return CacheKeys.CategoryGridVersion();
    }
}
