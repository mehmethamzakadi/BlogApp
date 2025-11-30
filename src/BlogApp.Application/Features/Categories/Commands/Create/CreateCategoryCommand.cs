using BlogApp.Application.Behaviors;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands.Create;

public sealed record CreateCategoryCommand(string Name, string? Description = null, Guid? ParentId = null) : IRequest<IResult>, IInvalidateCache
{
    public IEnumerable<string> GetCacheKeysToInvalidate()
    {
        yield return CacheKeys.CategoryGridVersion();
    }
}
