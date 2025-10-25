using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.UserEvents;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.EventHandlers;

/// <summary>
/// Handles UserRolesAssignedEvent and logs the activity
/// </summary>
public class UserRolesAssignedEventHandler : INotificationHandler<UserRolesAssignedEvent>
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserRolesAssignedEventHandler(
        IActivityLogRepository activityLogRepository,
        IUnitOfWork unitOfWork)
    {
        _activityLogRepository = activityLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UserRolesAssignedEvent notification, CancellationToken cancellationToken)
    {
        var rolesText = string.Join(", ", notification.RoleNames);

        var activityLog = new ActivityLog
        {
            ActivityType = "user_roles_assigned",
            EntityType = "User",
            EntityId = notification.UserId,
            Title = $"Kullanıcı \"{notification.UserName}\" için roller atandı",
            Details = $"Atanan Roller: {rolesText}",
            UserId = notification.AssignedById,
            Timestamp = DateTime.UtcNow
        };

        await _activityLogRepository.AddAsync(activityLog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
