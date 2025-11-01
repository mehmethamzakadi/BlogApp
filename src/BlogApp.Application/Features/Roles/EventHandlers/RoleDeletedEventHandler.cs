using BlogApp.Application.Abstractions;
using BlogApp.Domain.Events.RoleEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Roles.EventHandlers;

/// <summary>
/// Rol silindiğinde tetiklenen domain event handler
/// </summary>
public sealed class RoleDeletedEventHandler : INotificationHandler<RoleDeletedEvent>
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

    public async Task Handle(RoleDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling RoleDeletedEvent for Role {RoleId} - {RoleName}",
            notification.RoleId,
            notification.RoleName);

        try
        {
            await _cacheService.Remove($"role:{notification.RoleId}");
            await _cacheService.Remove($"role:{notification.RoleId}:permissions");
            await _cacheService.Remove("roles:list");
            await _cacheService.Remove("roles:all");

            _logger.LogInformation(
                "Cache invalidated for deleted role {RoleId}",
                notification.RoleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for RoleDeletedEvent {RoleId}",
                notification.RoleId);
        }
    }
}
