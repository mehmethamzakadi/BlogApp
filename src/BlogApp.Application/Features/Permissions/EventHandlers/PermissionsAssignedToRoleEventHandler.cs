using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Events.PermissionEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Permissions.EventHandlers;

/// <summary>
/// Role permission'lar atandığında tetiklenen domain event handler
/// </summary>
public sealed class PermissionsAssignedToRoleEventHandler : INotificationHandler<DomainEventNotification<PermissionsAssignedToRoleEvent>>
{
    private readonly ILogger<PermissionsAssignedToRoleEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public PermissionsAssignedToRoleEventHandler(
        ILogger<PermissionsAssignedToRoleEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<PermissionsAssignedToRoleEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling PermissionsAssignedToRoleEvent for Role {RoleId} ({RoleName}) - {PermissionCount} permissions: {PermissionNames}",
            domainEvent.RoleId,
            domainEvent.RoleName,
            domainEvent.PermissionNames.Count,
            string.Join(", ", domainEvent.PermissionNames));

        try
        {
            // ✅ FIXED: Use centralized CacheKeys instead of hardcoded strings
            // Role'ün permission cache'ini temizle
            await _cacheService.Remove(CacheKeys.RolePermissions(domainEvent.RoleId));
            await _cacheService.Remove(CacheKeys.Role(domainEvent.RoleId));
            
            // Note: Bu role sahip tüm user'ların permission cache'ini temizlemek gerekir
            // Ancak bu bilgiye burada erişemiyoruz - daha genel bir cache pattern kullanılabilir
            // Alternatif: Cache key pattern'i ile tüm user permission cache'lerini temizle
            // await _cacheService.RemoveByPattern("user:*:permissions");
            // Veya: UserListVersion() invalidate edilerek tüm user cache'leri yenilenebilir
            // Ancak bu çok agresif olabilir, sadece bu role sahip user'ları etkilemeli

            _logger.LogInformation(
                "Cache invalidated for role {RoleId} after permission assignment",
                domainEvent.RoleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for PermissionsAssignedToRoleEvent {RoleId}",
                domainEvent.RoleId);
        }

        // Gelecekte eklenebilecek side-effect'ler:
        // - Active session'ların permission'larını yenileme
        // - Audit log
    }
}
