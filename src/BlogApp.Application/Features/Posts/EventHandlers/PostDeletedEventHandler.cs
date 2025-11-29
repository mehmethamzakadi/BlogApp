using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Events.PostEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Posts.EventHandlers;

/// <summary>
/// Post silindiğinde tetiklenen domain event handler
/// </summary>
public class PostDeletedEventHandler : INotificationHandler<DomainEventNotification<PostDeletedEvent>>
{
    private readonly ILogger<PostDeletedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public PostDeletedEventHandler(
        ILogger<PostDeletedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<PostDeletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling PostDeletedEvent for Post {PostId} - {Title}",
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
            await _cacheService.Remove(CacheKeys.PostListVersion());

            // Note: CategoryId is not available in PostDeletedEvent
            // If needed, add CategoryId to the event in the future

            _logger.LogInformation(
                "Cache invalidated for deleted post {PostId}",
                domainEvent.PostId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for PostDeletedEvent {PostId}",
                domainEvent.PostId);
        }
    }
}
