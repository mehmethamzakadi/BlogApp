using BlogApp.Application.Abstractions;
using BlogApp.Domain.Events.PostEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Posts.EventHandlers;

/// <summary>
/// Post silindiğinde tetiklenen domain event handler
/// </summary>
public class PostDeletedEventHandler : INotificationHandler<PostDeletedEvent>
{
    private readonly ILogger<PostDeletedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public PostDeletedEventHandler(
        ILogger<PostDeletedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(PostDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling PostDeletedEvent for Post {PostId} - {Title}",
            notification.PostId,
            notification.Title);

        try
        {
            // Tüm ilgili cache'leri temizle
            await _cacheService.Remove($"post:{notification.PostId}");
            await _cacheService.Remove($"post:{notification.PostId}:withdrafts");
            await _cacheService.Remove("posts:recent");
            await _cacheService.Remove("posts:list");

            // Kategori cache'lerini de temizle
            // CategoryId event'te yok - gelecekte eklenebilir

            _logger.LogInformation(
                "Cache invalidated for deleted post {PostId}",
                notification.PostId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for PostDeletedEvent {PostId}",
                notification.PostId);
        }
    }
}
