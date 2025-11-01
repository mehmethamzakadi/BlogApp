using BlogApp.Application.Abstractions;
using BlogApp.Domain.Events.BookshelfItemEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.BookshelfItems.EventHandlers;

/// <summary>
/// BookshelfItem güncellendiğinde tetiklenen domain event handler
/// </summary>
public sealed class BookshelfItemUpdatedEventHandler : INotificationHandler<BookshelfItemUpdatedEvent>
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

    public async Task Handle(BookshelfItemUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling BookshelfItemUpdatedEvent for Item {ItemId} - {Title}",
            notification.ItemId,
            notification.Title);

        try
        {
            await _cacheService.Remove($"bookshelf:{notification.ItemId}");
            await _cacheService.Remove("bookshelf:list");
            await _cacheService.Remove("bookshelf:all");

            _logger.LogInformation(
                "Cache invalidated for bookshelf item {ItemId} after update",
                notification.ItemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for BookshelfItemUpdatedEvent {ItemId}",
                notification.ItemId);
        }
    }
}
