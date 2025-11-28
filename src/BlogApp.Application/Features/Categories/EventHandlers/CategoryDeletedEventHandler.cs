using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
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
            // Tüm ilgili cache'leri temizle
            await _cacheService.Remove($"category:{domainEvent.CategoryId}");
            await _cacheService.Remove($"category:{domainEvent.CategoryId}:posts");
            await _cacheService.Remove("categories:list");
            await _cacheService.Remove("categories:all");

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
