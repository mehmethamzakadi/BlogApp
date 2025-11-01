using BlogApp.Application.Abstractions;
using BlogApp.Domain.Events.UserEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Users.EventHandlers;

/// <summary>
/// Kullanıcıya roller atandığında tetiklenen domain event handler
/// </summary>
public sealed class UserRolesAssignedEventHandler : INotificationHandler<UserRolesAssignedEvent>
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

    public async Task Handle(UserRolesAssignedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling UserRolesAssignedEvent for User {UserId} ({UserName}) - {RoleCount} roles assigned: {RoleNames}",
            notification.UserId,
            notification.UserName,
            notification.RoleNames.Count,
            string.Join(", ", notification.RoleNames));

        try
        {
            // User'ın permission cache'ini temizle - roller değişti
            await _cacheService.Remove($"user:{notification.UserId}:roles");
            await _cacheService.Remove($"user:{notification.UserId}:permissions");
            await _cacheService.Remove($"user:{notification.UserId}");

            _logger.LogInformation(
                "Cache invalidated for user {UserId} after role assignment",
                notification.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for UserRolesAssignedEvent {UserId}",
                notification.UserId);
        }

        // Gelecekte eklenebilecek side-effect'ler:
        // - Active session'ların permission'larını yenileme
        // - Audit log
    }
}
