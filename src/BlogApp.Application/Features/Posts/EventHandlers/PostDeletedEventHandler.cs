using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Domain.Events.PostEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Posts.EventHandlers;

/// <summary>
/// Post silindiğinde tetiklenen domain event handler
/// </summary>
public class PostDeletedEventHandler : INotificationHandler<DomainEventNotification<PostDeletedEvent>>
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

    public async Task Handle(DomainEventNotification<PostDeletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling PostDeletedEvent for Post {PostId} - {Title}",
            domainEvent.PostId,
            domainEvent.Title);

        try
        {
            // Tüm ilgili cache'leri temizle
            await _cacheService.Remove($"post:{domainEvent.PostId}");
            await _cacheService.Remove($"post:{domainEvent.PostId}:withdrafts");
            await _cacheService.Remove("posts:recent");
            await _cacheService.Remove("posts:list");

            // Kategori cache'lerini de temizle
            // CategoryId event'te yok - gelecekte eklenebilir

            _logger.LogInformation(
                "Cache invalidated for deleted post {PostId}",
                domainEvent.PostId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for PostDeletedEvent {PostId}",
                domainEvent.PostId);
        }
    }
}
