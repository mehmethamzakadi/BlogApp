using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.PermissionEvents;

/// <summary>
/// Bir role yetkiler atandığında tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class PermissionsAssignedToRoleEvent : DomainEvent
{
    public int RoleId { get; }
    public string RoleName { get; }
    public IReadOnlyList<string> PermissionNames { get; }
    public int? AssignedById { get; }

    public PermissionsAssignedToRoleEvent(int roleId, string roleName, IReadOnlyList<string> permissionNames, int? assignedById)
    {
        RoleId = roleId;
        RoleName = roleName;
        PermissionNames = permissionNames;
        AssignedById = assignedById;
    }
}
