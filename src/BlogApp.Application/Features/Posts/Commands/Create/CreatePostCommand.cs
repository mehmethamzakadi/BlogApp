using BlogApp.Application.Behaviors;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Create;

public sealed record CreatePostCommand(string Title, string Body, string Summary, string Thumbnail, bool IsPublished, Guid CategoryId) : IRequest<IResult>, IInvalidateCache
{
    public IEnumerable<string> GetCacheKeysToInvalidate()
    {
        yield return CacheKeys.CategoryGridVersion();
    }
}
