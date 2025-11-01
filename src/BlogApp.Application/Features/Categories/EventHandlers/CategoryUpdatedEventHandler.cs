using BlogApp.Application.Abstractions;
using BlogApp.Domain.Events.CategoryEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Categories.EventHandlers;

/// <summary>
/// Kategori güncellendiğinde tetiklenen domain event handler
/// </summary>
public sealed class CategoryUpdatedEventHandler : INotificationHandler<CategoryUpdatedEvent>
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

    public async Task Handle(CategoryUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling CategoryUpdatedEvent for Category {CategoryId} - {Name}",
            notification.CategoryId,
            notification.Name);

        try
        {
            // Specific category cache'ini ve ilgili listeleri temizle
            await _cacheService.Remove($"category:{notification.CategoryId}");
            await _cacheService.Remove($"category:{notification.CategoryId}:posts");
            await _cacheService.Remove("categories:list");
            await _cacheService.Remove("categories:all");

            _logger.LogInformation(
                "Cache invalidated for category {CategoryId} after update",
                notification.CategoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for CategoryUpdatedEvent {CategoryId}",
                notification.CategoryId);
        }
    }
}
