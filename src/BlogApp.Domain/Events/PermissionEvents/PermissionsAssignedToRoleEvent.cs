using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.PermissionEvents;

[StoreInOutbox]
public class PermissionsAssignedToRoleEvent : DomainEvent
{
    public Guid RoleId { get; }
    public string RoleName { get; }
    public IReadOnlyList<string> PermissionNames { get; }

    public PermissionsAssignedToRoleEvent(Guid roleId, string roleName, IReadOnlyList<string> permissionNames)
    {
        RoleId = roleId;
        RoleName = roleName;
        PermissionNames = permissionNames;
    }
}