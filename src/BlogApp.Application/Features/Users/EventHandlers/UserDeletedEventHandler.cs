using BlogApp.Application.Abstractions;
using BlogApp.Domain.Events.UserEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Features.Users.EventHandlers;

/// <summary>
/// Kullanıcı silindiğinde tetiklenen domain event handler
/// </summary>
public sealed class UserDeletedEventHandler : INotificationHandler<UserDeletedEvent>
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

    public async Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling UserDeletedEvent for User {UserId}",
            notification.UserId);

        try
        {
            // Tüm ilgili cache'leri temizle
            await _cacheService.Remove($"user:{notification.UserId}");
            await _cacheService.Remove($"user:{notification.UserId}:roles");
            await _cacheService.Remove($"user:{notification.UserId}:permissions");
            await _cacheService.Remove("users:list");
            await _cacheService.Remove("users:all");
            await _cacheService.Remove("users:count");

            _logger.LogInformation(
                "Cache invalidated for deleted user {UserId}",
                notification.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error invalidating cache for UserDeletedEvent {UserId}",
                notification.UserId);
        }

        // Gelecekte eklenebilecek side-effect'ler:
        // - İlgili session'ları iptal etme
        // - Audit log oluşturma
        // - GDPR compliance cleanup
    }
}
