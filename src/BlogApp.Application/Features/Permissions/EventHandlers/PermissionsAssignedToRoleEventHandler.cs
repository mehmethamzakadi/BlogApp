using BlogApp.Application.Abstractions;
using BlogApp.Domain.Events.PermissionEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Permissions.EventHandlers;

/// <summary>
/// Role permission'lar atandığında tetiklenen domain event handler
/// </summary>
public sealed class PermissionsAssignedToRoleEventHandler : INotificationHandler<PermissionsAssignedToRoleEvent>
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

    public async Task Handle(PermissionsAssignedToRoleEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling PermissionsAssignedToRoleEvent for Role {RoleId} ({RoleName}) - {PermissionCount} permissions: {PermissionNames}",
            notification.RoleId,
            notification.RoleName,
            notification.PermissionNames.Count,
            string.Join(", ", notification.PermissionNames));

        try
        {
            // Role'ün permission cache'ini temizle
            await _cacheService.Remove($"role:{notification.RoleId}:permissions");
            await _cacheService.Remove($"role:{notification.RoleId}");
            
            // Bu role sahip tüm user'ların permission cache'ini temizlemek gerekir
            // Ancak bu bilgiye burada erişemiyoruz - daha genel bir cache pattern kullanılabilir
            // Alternatif: Cache key pattern'i ile tüm user permission cache'lerini temizle
            // await _cacheService.RemoveByPattern("user:*:permissions");

            _logger.LogInformation(
                "Cache invalidated for role {RoleId} after permission assignment",
                notification.RoleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for PermissionsAssignedToRoleEvent {RoleId}",
                notification.RoleId);
        }

        // Gelecekte eklenebilecek side-effect'ler:
        // - Active session'ların permission'larını yenileme
        // - Audit log
    }
}
