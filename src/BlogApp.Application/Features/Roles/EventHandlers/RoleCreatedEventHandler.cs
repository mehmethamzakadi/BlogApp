using BlogApp.Application.Abstractions;
using BlogApp.Domain.Events.RoleEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Roles.EventHandlers;

/// <summary>
/// Rol oluşturulduğunda tetiklenen domain event handler
/// </summary>
public sealed class RoleCreatedEventHandler : INotificationHandler<RoleCreatedEvent>
{
    private readonly ILogger<RoleCreatedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public RoleCreatedEventHandler(
        ILogger<RoleCreatedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(RoleCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling RoleCreatedEvent for Role {RoleId} - {RoleName}",
            notification.RoleId,
            notification.RoleName);

        try
        {
            // Cache invalidation
            await _cacheService.Remove("roles:list");
            await _cacheService.Remove("roles:all");

            _logger.LogInformation(
                "Cache invalidated after role {RoleId} creation",
                notification.RoleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for RoleCreatedEvent {RoleId}",
                notification.RoleId);
        }
    }
}
