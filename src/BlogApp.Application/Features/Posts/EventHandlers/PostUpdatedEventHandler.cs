using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Domain.Events.PostEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Posts.EventHandlers;

/// <summary>
/// Post güncellendiğinde tetiklenen domain event handler
/// </summary>
public class PostUpdatedEventHandler : INotificationHandler<DomainEventNotification<PostUpdatedEvent>>
{
    private readonly ILogger<PostUpdatedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public PostUpdatedEventHandler(
        ILogger<PostUpdatedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<PostUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling PostUpdatedEvent for Post {PostId} - {Title}",
            domainEvent.PostId,
            domainEvent.Title);

        try
        {
            // Specific post cache'ini ve ilgili listeleri temizle
            await _cacheService.Remove($"post:{domainEvent.PostId}");
            await _cacheService.Remove($"post:{domainEvent.PostId}:withdrafts");
            await _cacheService.Remove("posts:recent");
            await _cacheService.Remove("posts:list");

            _logger.LogInformation(
                "Cache invalidated for post {PostId} after update",
                domainEvent.PostId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for PostUpdatedEvent {PostId}",
                domainEvent.PostId);
        }
    }
}
