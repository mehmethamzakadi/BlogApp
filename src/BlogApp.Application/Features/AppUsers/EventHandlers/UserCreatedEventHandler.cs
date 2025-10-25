using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.UserEvents;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.EventHandlers;

/// <summary>
/// Handles UserCreatedEvent and logs the activity
/// </summary>
public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserCreatedEventHandler(
        IActivityLogRepository activityLogRepository,
        IUnitOfWork unitOfWork)
    {
        _activityLogRepository = activityLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        var activityLog = new ActivityLog
        {
            ActivityType = "user_created",
            EntityType = "User",
            EntityId = notification.UserId,
            Title = $"Kullanıcı \"{notification.UserName}\" oluşturuldu",
            Details = $"Email: {notification.Email}",
            UserId = notification.CreatedById,
            Timestamp = DateTime.UtcNow
        };

        await _activityLogRepository.AddAsync(activityLog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
