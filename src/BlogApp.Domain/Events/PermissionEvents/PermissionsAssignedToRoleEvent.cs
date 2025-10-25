using BlogApp.Domain.Common;

namespace BlogApp.Domain.Events.PermissionEvents;

/// <summary>
/// Domain event raised when permissions are assigned to a role
/// </summary>
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
