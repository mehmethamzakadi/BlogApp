using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Events.RoleEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Roles.EventHandlers;

/// <summary>
/// Rol silindiğinde tetiklenen domain event handler
/// </summary>
public sealed class RoleDeletedEventHandler : INotificationHandler<DomainEventNotification<RoleDeletedEvent>>
{
    private readonly ILogger<RoleDeletedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public RoleDeletedEventHandler(
        ILogger<RoleDeletedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<RoleDeletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling RoleDeletedEvent for Role {RoleId} - {RoleName}",
            domainEvent.RoleId,
            domainEvent.RoleName);

        try
        {
            // ✅ FIXED: Use centralized CacheKeys instead of hardcoded strings
            // Invalidate specific role caches
            await _cacheService.Remove(CacheKeys.Role(domainEvent.RoleId));
            await _cacheService.Remove(CacheKeys.RolePermissions(domainEvent.RoleId));
            
            // Invalidate role list version to invalidate all cached role lists
            await _cacheService.Remove(CacheKeys.RoleListVersion());

            _logger.LogInformation(
                "Cache invalidated for deleted role {RoleId}",
                domainEvent.RoleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for RoleDeletedEvent {RoleId}",
                domainEvent.RoleId);
        }
    }
}
