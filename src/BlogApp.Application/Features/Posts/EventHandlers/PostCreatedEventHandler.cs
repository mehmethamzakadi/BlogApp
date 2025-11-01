using BlogApp.Application.Abstractions;
using BlogApp.Domain.Events.PostEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Posts.EventHandlers;

/// <summary>
/// Post oluşturulduğunda tetiklenen domain event handler
/// Cache invalidation ve side-effect'leri yönetir
/// </summary>
public class PostCreatedEventHandler : INotificationHandler<PostCreatedEvent>
{
    private readonly ILogger<PostCreatedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public PostCreatedEventHandler(
        ILogger<PostCreatedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(PostCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling PostCreatedEvent for Post {PostId} - {Title}",
            notification.PostId,
            notification.Title);

        try
        {
            // Cache invalidation - kategori bazlı post listelerini temizle
            await _cacheService.Remove($"category:{notification.CategoryId}:posts");
            await _cacheService.Remove("posts:recent");
            await _cacheService.Remove("posts:list");

            _logger.LogInformation(
                "Cache invalidated for category {CategoryId} after post creation",
                notification.CategoryId);
        }
        catch (Exception ex)
        {
            // Cache hatası kritik değil, log ve devam et
            _logger.LogError(ex,
                "Error invalidating cache for PostCreatedEvent {PostId}",
                notification.PostId);
        }

        // Gelecekte eklenebilecek side-effect'ler:
        // - Search index güncelleme
        // - Recommendation engine'e bildirim
        // - Analytics event'i
    }
}
