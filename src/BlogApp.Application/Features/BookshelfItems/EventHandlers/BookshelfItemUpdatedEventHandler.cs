using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Events.BookshelfItemEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.BookshelfItems.EventHandlers;

/// <summary>
/// BookshelfItem güncellendiğinde tetiklenen domain event handler
/// </summary>
public sealed class BookshelfItemUpdatedEventHandler : INotificationHandler<DomainEventNotification<BookshelfItemUpdatedEvent>>
{
    private readonly ILogger<BookshelfItemUpdatedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public BookshelfItemUpdatedEventHandler(
        ILogger<BookshelfItemUpdatedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<BookshelfItemUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling BookshelfItemUpdatedEvent for Item {ItemId} - {Title}",
            domainEvent.ItemId,
            domainEvent.Title);

        try
        {
            // ✅ FIXED: Use centralized CacheKeys instead of hardcoded strings
            // Invalidate specific bookshelf item cache
            await _cacheService.Remove(CacheKeys.BookshelfItem(domainEvent.ItemId));
            
            // Invalidate bookshelf list version to invalidate all cached bookshelf lists
            await _cacheService.Remove(CacheKeys.BookshelfListVersion());

            _logger.LogInformation(
                "Cache invalidated for bookshelf item {ItemId} after update",
                domainEvent.ItemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for BookshelfItemUpdatedEvent {ItemId}",
                domainEvent.ItemId);
        }
    }
}
