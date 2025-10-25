using Microsoft.AspNetCore.Authorization;

namespace BlogApp.Infrastructure.Authorization;

/// <summary>
/// Permission bazlı yetkilendirme için requirement sınıfı
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Gereken permission adı (örn: "Users.Create", "Posts.Delete")
    /// </summary>
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
