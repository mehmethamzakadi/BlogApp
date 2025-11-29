using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Events.CategoryEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Categories.EventHandlers;

/// <summary>
/// Kategori oluşturulduğunda tetiklenen domain event handler
/// Cache invalidation ve side-effect'leri yönetir
/// </summary>
public sealed class CategoryCreatedEventHandler : INotificationHandler<DomainEventNotification<CategoryCreatedEvent>>
{
    private readonly ILogger<CategoryCreatedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public CategoryCreatedEventHandler(
        ILogger<CategoryCreatedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<CategoryCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling CategoryCreatedEvent for Category {CategoryId} - {Name}",
            domainEvent.CategoryId,
            domainEvent.Name);

        try
        {
            // ✅ FIXED: Use centralized CacheKeys instead of hardcoded strings
            // Invalidate category list version to invalidate all cached category lists
            await _cacheService.Remove(CacheKeys.CategoryListVersion());
            
            // Also invalidate category grid version
            await _cacheService.Remove(CacheKeys.CategoryGridVersion());

            _logger.LogInformation(
                "Cache invalidated after category {CategoryId} creation",
                domainEvent.CategoryId);
        }
        catch (Exception ex)
        {
            // Cache hatası kritik değil, log ve devam et
            _logger.LogError(ex,
                "Error invalidating cache for CategoryCreatedEvent {CategoryId}",
                domainEvent.CategoryId);
        }

        // Gelecekte eklenebilecek side-effect'ler:
        // - Analytics event'i
        // - Notification gönderme
    }
}
