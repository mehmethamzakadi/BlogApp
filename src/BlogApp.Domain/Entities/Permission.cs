using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

/// <summary>
/// Sistemdeki permission'ları temsil eder.
/// Her permission bir modül ve aksiyon kombinasyonudur (örn: Users.Create, Posts.Delete)
/// </summary>
public sealed class Permission : BaseEntity
{
    /// <summary>
    /// Permission'ın benzersiz adı (örn: "Users.Create", "Posts.Delete")
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Normalize edilmiş permission adı (case-insensitive arama için)
    /// </summary>
    public string? NormalizedName { get; set; }

    /// <summary>
    /// Permission'ın açıklaması
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Hangi modüle ait olduğu (Users, Posts, Categories vb.)
    /// </summary>
    public required string Module { get; set; }

    /// <summary>
    /// Permission tipi (Create, Read, Update, Delete vb.)
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Bu permission'a sahip roller
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
