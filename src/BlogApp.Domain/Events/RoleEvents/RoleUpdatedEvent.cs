using BlogApp.Domain.Common;

namespace BlogApp.Domain.Events.RoleEvents;

/// <summary>
/// Domain event raised when a role is updated
/// </summary>
public class RoleUpdatedEvent : DomainEvent
{
    public int RoleId { get; }
    public string RoleName { get; }
    public int? UpdatedById { get; }

    public RoleUpdatedEvent(int roleId, string roleName, int? updatedById)
    {
        RoleId = roleId;
        RoleName = roleName;
        UpdatedById = updatedById;
    }
}
