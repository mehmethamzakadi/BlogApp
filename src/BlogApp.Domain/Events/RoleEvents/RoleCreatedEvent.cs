using BlogApp.Domain.Common;

namespace BlogApp.Domain.Events.RoleEvents;

/// <summary>
/// Domain event raised when a new role is created
/// </summary>
public class RoleCreatedEvent : DomainEvent
{
    public int RoleId { get; }
    public string RoleName { get; }
    public int? CreatedById { get; }

    public RoleCreatedEvent(int roleId, string roleName, int? createdById)
    {
        RoleId = roleId;
        RoleName = roleName;
        CreatedById = createdById;
    }
}
