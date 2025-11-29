using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Events.CategoryEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Categories.EventHandlers;

/// <summary>
/// Kategori güncellendiğinde tetiklenen domain event handler
/// </summary>
public sealed class CategoryUpdatedEventHandler : INotificationHandler<DomainEventNotification<CategoryUpdatedEvent>>
{
    private readonly ILogger<CategoryUpdatedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public CategoryUpdatedEventHandler(
        ILogger<CategoryUpdatedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<CategoryUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling CategoryUpdatedEvent for Category {CategoryId} - {Name}",
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

            _logger.LogInformation(
                "Cache invalidated for category {CategoryId} after update",
                domainEvent.CategoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for CategoryUpdatedEvent {CategoryId}",
                domainEvent.CategoryId);
        }
    }
}
