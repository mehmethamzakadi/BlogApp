using BlogApp.Application.Behaviors;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Delete;

public sealed record DeletePostCommand(Guid Id) : IRequest<IResult>, IInvalidateCache
{
    public IEnumerable<string> GetCacheKeysToInvalidate()
    {
        yield return CacheKeys.PostPublic(Id);
        yield return CacheKeys.PostWithDrafts(Id);
        yield return CacheKeys.CategoryGridVersion();
    }
}
