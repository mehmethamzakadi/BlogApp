using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Domain.Events.BookshelfItemEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.BookshelfItems.EventHandlers;

/// <summary>
/// BookshelfItem silindiğinde tetiklenen domain event handler
/// </summary>
public sealed class BookshelfItemDeletedEventHandler : INotificationHandler<DomainEventNotification<BookshelfItemDeletedEvent>>
{
    private readonly ILogger<BookshelfItemDeletedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public BookshelfItemDeletedEventHandler(
        ILogger<BookshelfItemDeletedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<BookshelfItemDeletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling BookshelfItemDeletedEvent for Item {ItemId} - {Title}",
            domainEvent.ItemId,
            domainEvent.Title);

        try
        {
            await _cacheService.Remove($"bookshelf:{domainEvent.ItemId}");
            await _cacheService.Remove("bookshelf:list");
            await _cacheService.Remove("bookshelf:all");

            _logger.LogInformation(
                "Cache invalidated for deleted bookshelf item {ItemId}",
                domainEvent.ItemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for BookshelfItemDeletedEvent {ItemId}",
                domainEvent.ItemId);
        }
    }
}
