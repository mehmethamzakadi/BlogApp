using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.PermissionEvents;

/// <summary>
/// Bir role yetkiler atandığında tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class PermissionsAssignedToRoleEvent : DomainEvent
{
    public Guid RoleId { get; }
    public string RoleName { get; }
    public IReadOnlyList<string> PermissionNames { get; }
    public Guid? AssignedById { get; }

    public PermissionsAssignedToRoleEvent(Guid roleId, string roleName, IReadOnlyList<string> permissionNames, Guid? assignedById)
    {
        RoleId = roleId;
        RoleName = roleName;
        PermissionNames = permissionNames;
        AssignedById = assignedById;
    }
}
