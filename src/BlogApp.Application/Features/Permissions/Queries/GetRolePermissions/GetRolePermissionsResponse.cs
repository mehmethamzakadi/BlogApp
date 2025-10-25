namespace BlogApp.Application.Features.Permissions.Queries.GetRolePermissions;

public class GetRolePermissionsResponse
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public List<int> PermissionIds { get; set; } = new();
}
