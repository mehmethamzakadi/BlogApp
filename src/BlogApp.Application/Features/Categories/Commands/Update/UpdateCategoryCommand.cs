using BlogApp.Application.Behaviors;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands.Update;

public sealed record UpdateCategoryCommand(Guid Id, string Name) : IRequest<IResult>, IInvalidateCache
{
    public IEnumerable<string> GetCacheKeysToInvalidate()
    {
        yield return CacheKeys.Category(Id);
        yield return CacheKeys.CategoryGridVersion();
    }
}
