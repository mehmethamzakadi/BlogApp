using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Events.PostEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Posts.EventHandlers;

/// <summary>
/// Post oluşturulduğunda tetiklenen domain event handler
/// Cache invalidation ve side-effect'leri yönetir
/// </summary>
public class PostCreatedEventHandler : INotificationHandler<DomainEventNotification<PostCreatedEvent>>
{
    private readonly ILogger<PostCreatedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public PostCreatedEventHandler(
        ILogger<PostCreatedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<PostCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling PostCreatedEvent for Post {PostId} - {Title}",
            domainEvent.PostId,
            domainEvent.Title);

        try
        {
            // ✅ FIXED: Use centralized CacheKeys instead of hardcoded strings
            // Invalidate post list version to invalidate all cached post lists
            await _cacheService.Remove(CacheKeys.PostListVersion());
            
            // Invalidate category-specific post list version
            await _cacheService.Remove(CacheKeys.PostsByCategoryVersion(domainEvent.CategoryId));

            _logger.LogInformation(
                "Cache invalidated for category {CategoryId} after post creation",
                domainEvent.CategoryId);
        }
        catch (Exception ex)
        {
            // Cache hatası kritik değil, log ve devam et
            _logger.LogError(ex,
                "Error invalidating cache for PostCreatedEvent {PostId}",
                domainEvent.PostId);
        }

        // Gelecekte eklenebilecek side-effect'ler:
        // - Search index güncelleme
        // - Recommendation engine'e bildirim
        // - Analytics event'i
    }
}
