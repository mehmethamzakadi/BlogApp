using BlogApp.Domain.Common;

namespace BlogApp.Domain.Events.RoleEvents;

/// <summary>
/// Domain event raised when a role is deleted
/// </summary>
public class RoleDeletedEvent : DomainEvent
{
    public int RoleId { get; }
    public string RoleName { get; }
    public int? DeletedById { get; }

    public RoleDeletedEvent(int roleId, string roleName, int? deletedById)
    {
        RoleId = roleId;
        RoleName = roleName;
        DeletedById = deletedById;
    }
}
