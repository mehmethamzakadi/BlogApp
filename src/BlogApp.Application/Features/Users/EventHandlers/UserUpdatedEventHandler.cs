using BlogApp.Application.Abstractions;
using BlogApp.Domain.Events.UserEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Users.EventHandlers;

/// <summary>
/// Kullanıcı güncellendiğinde tetiklenen domain event handler
/// </summary>
public sealed class UserUpdatedEventHandler : INotificationHandler<UserUpdatedEvent>
{
    private readonly ILogger<UserUpdatedEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public UserUpdatedEventHandler(
        ILogger<UserUpdatedEventHandler> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Handle(UserUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling UserUpdatedEvent for User {UserId}",
            notification.UserId);

        try
        {
            // Specific user cache'ini ve ilgili listeleri temizle
            await _cacheService.Remove($"user:{notification.UserId}");
            await _cacheService.Remove($"user:{notification.UserId}:roles");
            await _cacheService.Remove($"user:{notification.UserId}:permissions");
            await _cacheService.Remove("users:list");
            await _cacheService.Remove("users:all");

            _logger.LogInformation(
                "Cache invalidated for user {UserId} after update",
                notification.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for UserUpdatedEvent {UserId}",
                notification.UserId);
        }
    }
}
