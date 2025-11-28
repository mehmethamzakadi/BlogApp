using BlogApp.Application.Abstractions;
using BlogApp.Application.Common;
using BlogApp.Domain.Events.RoleEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Roles.EventHandlers;

/// <summary>
/// Rol güncellendiğinde tetiklenen domain event handler
/// </summary>
public sealed class RoleUpdatedEventHandler : INotificationHandler<DomainEventNotification<RoleUpdatedEvent>>
{
    private readonly ILogger<RoleUpdatedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public RoleUpdatedEventHandler(
        ILogger<RoleUpdatedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<RoleUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Handling RoleUpdatedEvent for Role {RoleId} - {RoleName}",
            domainEvent.RoleId,
            domainEvent.RoleName);

        try
        {
            await _cacheService.Remove($"role:{domainEvent.RoleId}");
            await _cacheService.Remove($"role:{domainEvent.RoleId}:permissions");
            await _cacheService.Remove("roles:list");
            await _cacheService.Remove("roles:all");

            _logger.LogInformation(
                "Cache invalidated for role {RoleId} after update",
                domainEvent.RoleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for RoleUpdatedEvent {RoleId}",
                domainEvent.RoleId);
        }
    }
}
