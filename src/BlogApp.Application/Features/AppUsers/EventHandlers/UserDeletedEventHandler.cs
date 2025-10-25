using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.UserEvents;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.EventHandlers;

/// <summary>
/// Handles UserDeletedEvent and logs the activity
/// </summary>
public class UserDeletedEventHandler : INotificationHandler<UserDeletedEvent>
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserDeletedEventHandler(
        IActivityLogRepository activityLogRepository,
        IUnitOfWork unitOfWork)
    {
        _activityLogRepository = activityLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
    {
        var activityLog = new ActivityLog
        {
            ActivityType = "user_deleted",
            EntityType = "User",
            EntityId = notification.UserId,
            Title = $"Kullanıcı \"{notification.UserName}\" silindi",
            Details = $"Email: {notification.Email}",
            UserId = notification.DeletedById,
            Timestamp = DateTime.UtcNow
        };

        await _activityLogRepository.AddAsync(activityLog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
