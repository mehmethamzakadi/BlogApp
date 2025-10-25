using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.PermissionEvents;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Permissions.EventHandlers;

/// <summary>
/// Handles PermissionsAssignedToRoleEvent and logs the activity
/// </summary>
public class PermissionsAssignedToRoleEventHandler : INotificationHandler<PermissionsAssignedToRoleEvent>
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PermissionsAssignedToRoleEventHandler(
        IActivityLogRepository activityLogRepository,
        IUnitOfWork unitOfWork)
    {
        _activityLogRepository = activityLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(PermissionsAssignedToRoleEvent notification, CancellationToken cancellationToken)
    {
        var permissionsText = string.Join(", ", notification.PermissionNames);

        var activityLog = new ActivityLog
        {
            ActivityType = "permissions_assigned_to_role",
            EntityType = "Role",
            EntityId = notification.RoleId,
            Title = $"\"{notification.RoleName}\" rolüne yetkiler atandı",
            Details = $"Atanan Yetkiler: {permissionsText}",
            UserId = notification.AssignedById,
            Timestamp = DateTime.UtcNow
        };

        await _activityLogRepository.AddAsync(activityLog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
