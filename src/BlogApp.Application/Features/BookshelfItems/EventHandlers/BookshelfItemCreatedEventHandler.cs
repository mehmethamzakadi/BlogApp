using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Events.BookshelfItemEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.BookshelfItems.EventHandlers;

/// <summary>
/// BookshelfItem oluşturulduğunda tetiklenen domain event handler
/// </summary>
public sealed class BookshelfItemCreatedEventHandler : INotificationHandler<DomainEventNotification<BookshelfItemCreatedEvent>>
{
    private readonly ILogger<BookshelfItemCreatedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public BookshelfItemCreatedEventHandler(
        ILogger<BookshelfItemCreatedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<BookshelfItemCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling BookshelfItemCreatedEvent for Item {ItemId} - {Title}",
            domainEvent.ItemId,
            domainEvent.Title);

        try
        {
            // ✅ FIXED: Use centralized CacheKeys instead of hardcoded strings
            // Invalidate bookshelf list version to invalidate all cached bookshelf lists
            await _cacheService.Remove(CacheKeys.BookshelfListVersion());

            _logger.LogInformation(
                "Cache invalidated after bookshelf item {ItemId} creation",
                domainEvent.ItemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for BookshelfItemCreatedEvent {ItemId}",
                domainEvent.ItemId);
        }
    }
}
