using BlogApp.Application.Abstractions;
using BlogApp.Domain.Events.CategoryEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Categories.EventHandlers;

/// <summary>
/// Kategori silindiğinde tetiklenen domain event handler
/// </summary>
public sealed class CategoryDeletedEventHandler : INotificationHandler<CategoryDeletedEvent>
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

    public async Task Handle(CategoryDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling CategoryDeletedEvent for Category {CategoryId} - {Name}",
            notification.CategoryId,
            notification.Name);

        try
        {
            // Tüm ilgili cache'leri temizle
            await _cacheService.Remove($"category:{notification.CategoryId}");
            await _cacheService.Remove($"category:{notification.CategoryId}:posts");
            await _cacheService.Remove("categories:list");
            await _cacheService.Remove("categories:all");

            _logger.LogInformation(
                "Cache invalidated for deleted category {CategoryId}",
                notification.CategoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for CategoryDeletedEvent {CategoryId}",
                notification.CategoryId);
        }
    }
}
