namespace BlogApp.Application.Features.Permissions.Queries.GetRolePermissions;

public class GetRolePermissionsResponse
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public List<Guid> PermissionIds { get; set; } = new();
}
