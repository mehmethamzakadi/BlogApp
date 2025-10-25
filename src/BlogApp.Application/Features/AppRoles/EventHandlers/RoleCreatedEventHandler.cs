using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.RoleEvents;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.EventHandlers;

/// <summary>
/// Handles RoleCreatedEvent and logs the activity
/// </summary>
public class RoleCreatedEventHandler : INotificationHandler<RoleCreatedEvent>
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RoleCreatedEventHandler(
        IActivityLogRepository activityLogRepository,
        IUnitOfWork unitOfWork)
    {
        _activityLogRepository = activityLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RoleCreatedEvent notification, CancellationToken cancellationToken)
    {
        var activityLog = new ActivityLog
        {
            ActivityType = "role_created",
            EntityType = "Role",
            EntityId = notification.RoleId,
            Title = $"Rol \"{notification.RoleName}\" oluşturuldu",
            UserId = notification.CreatedById,
            Timestamp = DateTime.UtcNow
        };

        await _activityLogRepository.AddAsync(activityLog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
