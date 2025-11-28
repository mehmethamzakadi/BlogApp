using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Domain.Events.UserEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Users.EventHandlers;

/// <summary>
/// Kullanıcı oluşturulduğunda tetiklenen domain event handler
/// </summary>
public sealed class UserCreatedEventHandler : INotificationHandler<DomainEventNotification<UserCreatedEvent>>
{
    private readonly ILogger<UserCreatedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public UserCreatedEventHandler(
        ILogger<UserCreatedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<UserCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling UserCreatedEvent for User {UserId} - {Email}",
            domainEvent.UserId,
            domainEvent.Email);

        try
        {
            // Cache invalidation - kullanıcı listelerini temizle
            await _cacheService.Remove("users:list");
            await _cacheService.Remove("users:all");
            await _cacheService.Remove("users:count");

            _logger.LogInformation(
                "Cache invalidated after user {UserId} creation",
                domainEvent.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for UserCreatedEvent {UserId}",
                domainEvent.UserId);
        }

        // Gelecekte eklenebilecek side-effect'ler:
        // - Welcome email gönderme
        // - Default role atama (eğer domain'de yapılmıyorsa)
        // - Analytics event'i
    }
}
