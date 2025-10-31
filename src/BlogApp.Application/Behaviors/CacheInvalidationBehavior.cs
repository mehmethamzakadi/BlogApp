using BlogApp.Application.Abstractions;
using MediatR;

namespace BlogApp.Application.Behaviors;

public sealed class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICacheService _cacheService;

    public CacheInvalidationBehavior(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        if (request is IInvalidateCache invalidator)
        {
            foreach (var cacheKey in invalidator.GetCacheKeysToInvalidate())
            {
                await _cacheService.Remove(cacheKey);
            }
        }

        return response;
    }
}

public interface IInvalidateCache
{
    IEnumerable<string> GetCacheKeysToInvalidate();
}
