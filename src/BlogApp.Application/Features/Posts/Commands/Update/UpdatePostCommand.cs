using BlogApp.Application.Behaviors;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Update;

public sealed record UpdatePostCommand(Guid Id, string Title, string Body, string Summary, string Thumbnail, bool IsPublished, Guid CategoryId) : IRequest<IResult>, IInvalidateCache
{
    public IEnumerable<string> GetCacheKeysToInvalidate()
    {
        yield return CacheKeys.Post(Id);
        yield return CacheKeys.PostPublic(Id);
        yield return CacheKeys.PostWithDrafts(Id);
        yield return CacheKeys.CategoryGridVersion();
        
        for (int i = 0; i < 10; i++)
        {
            yield return CacheKeys.PostList(i, 10);
            yield return CacheKeys.PostsByCategory(CategoryId, i, 10);
        }
    }
}
