using Microsoft.AspNetCore.Identity;

namespace BlogApp.Domain.Entities;

public sealed class AppRole : IdentityRole<int>
{
    /// <summary>
    /// Bu role atanmış permission'lar
    /// </summary>
    public ICollection<AppRolePermission> RolePermissions { get; set; } = new List<AppRolePermission>();
}
