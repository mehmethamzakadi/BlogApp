using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Events.CategoryEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Categories.EventHandlers;

/// <summary>
/// Kategori silindiğinde tetiklenen domain event handler
/// </summary>
public sealed class CategoryDeletedEventHandler : INotificationHandler<DomainEventNotification<CategoryDeletedEvent>>
{
    private readonly ILogger<CategoryDeletedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public CategoryDeletedEventHandler(
        ILogger<CategoryDeletedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<CategoryDeletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling CategoryDeletedEvent for Category {CategoryId} - {Name}",
            domainEvent.CategoryId,
            domainEvent.Name);

        try
        {
            // ✅ FIXED: Use centralized CacheKeys instead of hardcoded strings
            // Invalidate specific category caches
            await _cacheService.Remove(CacheKeys.Category(domainEvent.CategoryId));
            await _cacheService.Remove(CacheKeys.CategoryPosts(domainEvent.CategoryId));
            
            // Invalidate category list version to invalidate all cached category lists
            await _cacheService.Remove(CacheKeys.CategoryListVersion());
            
            // Also invalidate category grid version
            await _cacheService.Remove(CacheKeys.CategoryGridVersion());
            
            // Invalidate posts by category version
            await _cacheService.Remove(CacheKeys.PostsByCategoryVersion(domainEvent.CategoryId));

            _logger.LogInformation(
                "Cache invalidated for deleted category {CategoryId}",
                domainEvent.CategoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for CategoryDeletedEvent {CategoryId}",
                domainEvent.CategoryId);
        }
    }
}
