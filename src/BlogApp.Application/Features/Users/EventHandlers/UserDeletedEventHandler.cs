using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Events.UserEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Users.EventHandlers;

/// <summary>
/// Kullanıcı silindiğinde tetiklenen domain event handler
/// </summary>
public sealed class UserDeletedEventHandler : INotificationHandler<DomainEventNotification<UserDeletedEvent>>
{
    private readonly ILogger<UserDeletedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public UserDeletedEventHandler(
        ILogger<UserDeletedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<UserDeletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling UserDeletedEvent for User {UserId}",
            domainEvent.UserId);

        try
        {
            // ✅ FIXED: Use centralized CacheKeys instead of hardcoded strings
            // Invalidate specific user caches
            await _cacheService.Remove(CacheKeys.User(domainEvent.UserId));
            await _cacheService.Remove(CacheKeys.UserRoles(domainEvent.UserId));
            await _cacheService.Remove(CacheKeys.UserPermissions(domainEvent.UserId));
            
            // Invalidate user list version to invalidate all cached user lists
            await _cacheService.Remove(CacheKeys.UserListVersion());

            _logger.LogInformation(
                "Cache invalidated for deleted user {UserId}",
                domainEvent.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for UserDeletedEvent {UserId}",
                domainEvent.UserId);
        }

        // Gelecekte eklenebilecek side-effect'ler:
        // - İlgili session'ları iptal etme
        // - Audit log oluşturma
        // - GDPR compliance cleanup
    }
}
