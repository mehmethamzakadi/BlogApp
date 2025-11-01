using BlogApp.Application.Abstractions;
using BlogApp.Domain.Events.BookshelfItemEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.BookshelfItems.EventHandlers;

/// <summary>
/// BookshelfItem oluşturulduğunda tetiklenen domain event handler
/// </summary>
public sealed class BookshelfItemCreatedEventHandler : INotificationHandler<BookshelfItemCreatedEvent>
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

    public async Task Handle(BookshelfItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling BookshelfItemCreatedEvent for Item {ItemId} - {Title}",
            notification.ItemId,
            notification.Title);

        try
        {
            // Cache invalidation
            await _cacheService.Remove("bookshelf:list");
            await _cacheService.Remove("bookshelf:all");

            _logger.LogInformation(
                "Cache invalidated after bookshelf item {ItemId} creation",
                notification.ItemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for BookshelfItemCreatedEvent {ItemId}",
                notification.ItemId);
        }
    }
}
