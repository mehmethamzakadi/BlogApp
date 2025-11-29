using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Events.PostEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Posts.EventHandlers;

/// <summary>
/// Post güncellendiğinde tetiklenen domain event handler
/// </summary>
public class PostUpdatedEventHandler : INotificationHandler<DomainEventNotification<PostUpdatedEvent>>
{
    private readonly ILogger<PostUpdatedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public PostUpdatedEventHandler(
        ILogger<PostUpdatedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<PostUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling PostUpdatedEvent for Post {PostId} - {Title}",
            domainEvent.PostId,
            domainEvent.Title);

        try
        {
            // ✅ FIXED: Use centralized CacheKeys instead of hardcoded strings
            // Invalidate specific post caches
            await _cacheService.Remove(CacheKeys.Post(domainEvent.PostId));
            await _cacheService.Remove(CacheKeys.PostPublic(domainEvent.PostId));
            await _cacheService.Remove(CacheKeys.PostWithDrafts(domainEvent.PostId));
            
            // Invalidate post list version to invalidate all cached post lists
            // This is more efficient than removing individual list cache entries
            await _cacheService.Remove(CacheKeys.PostListVersion());

            _logger.LogInformation(
                "Cache invalidated for post {PostId} after update",
                domainEvent.PostId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for PostUpdatedEvent {PostId}",
                domainEvent.PostId);
        }
    }
}
