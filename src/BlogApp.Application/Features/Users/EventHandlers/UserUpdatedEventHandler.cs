using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Domain.Events.UserEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Users.EventHandlers;

/// <summary>
/// Kullanıcı güncellendiğinde tetiklenen domain event handler
/// </summary>
public sealed class UserUpdatedEventHandler : INotificationHandler<DomainEventNotification<UserUpdatedEvent>>
{
    private readonly ILogger<UserUpdatedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public UserUpdatedEventHandler(
        ILogger<UserUpdatedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<UserUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling UserUpdatedEvent for User {UserId}",
            domainEvent.UserId);

        try
        {
            // Specific user cache'ini ve ilgili listeleri temizle
            await _cacheService.Remove($"user:{domainEvent.UserId}");
            await _cacheService.Remove($"user:{domainEvent.UserId}:roles");
            await _cacheService.Remove($"user:{domainEvent.UserId}:permissions");
            await _cacheService.Remove("users:list");
            await _cacheService.Remove("users:all");

            _logger.LogInformation(
                "Cache invalidated for user {UserId} after update",
                domainEvent.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for UserUpdatedEvent {UserId}",
                domainEvent.UserId);
        }
    }
}
