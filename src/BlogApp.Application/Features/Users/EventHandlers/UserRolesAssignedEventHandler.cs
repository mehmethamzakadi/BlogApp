using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Events.UserEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Users.EventHandlers;

/// <summary>
/// Kullanıcıya roller atandığında tetiklenen domain event handler
/// </summary>
public sealed class UserRolesAssignedEventHandler : INotificationHandler<DomainEventNotification<UserRolesAssignedEvent>>
{
    private readonly ILogger<UserRolesAssignedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public UserRolesAssignedEventHandler(
        ILogger<UserRolesAssignedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<UserRolesAssignedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling UserRolesAssignedEvent for User {UserId} ({UserName}) - {RoleCount} roles assigned: {RoleNames}",
            domainEvent.UserId,
            domainEvent.UserName,
            domainEvent.RoleNames.Count,
            string.Join(", ", domainEvent.RoleNames));

        try
        {
            // ✅ FIXED: Use centralized CacheKeys instead of hardcoded strings
            // User'ın permission cache'ini temizle - roller değişti
            await _cacheService.Remove(CacheKeys.UserRoles(domainEvent.UserId));
            await _cacheService.Remove(CacheKeys.UserPermissions(domainEvent.UserId));
            await _cacheService.Remove(CacheKeys.User(domainEvent.UserId));

            _logger.LogInformation(
                "Cache invalidated for user {UserId} after role assignment",
                domainEvent.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for UserRolesAssignedEvent {UserId}",
                domainEvent.UserId);
        }

        // Gelecekte eklenebilecek side-effect'ler:
        // - Active session'ların permission'larını yenileme
        // - Audit log
    }
}
