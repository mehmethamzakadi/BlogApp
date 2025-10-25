using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.UserEvents;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.EventHandlers;

/// <summary>
/// Handles UserUpdatedEvent and logs the activity
/// </summary>
public class UserUpdatedEventHandler : INotificationHandler<UserUpdatedEvent>
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserUpdatedEventHandler(
        IActivityLogRepository activityLogRepository,
        IUnitOfWork unitOfWork)
    {
        _activityLogRepository = activityLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UserUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var activityLog = new ActivityLog
        {
            ActivityType = "user_updated",
            EntityType = "User",
            EntityId = notification.UserId,
            Title = $"Kullanıcı \"{notification.UserName}\" güncellendi",
            Details = $"Email: {notification.Email}",
            UserId = notification.UpdatedById,
            Timestamp = DateTime.UtcNow
        };

        await _activityLogRepository.AddAsync(activityLog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
