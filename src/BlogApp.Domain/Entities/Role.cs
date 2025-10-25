using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

/// <summary>
/// Sistemdeki rolleri temsil eder.
/// Identity'den bağımsız, custom role entity.
/// </summary>
public sealed class Role : BaseEntity
{
    /// <summary>
    /// Rol adı (benzersiz olmalı, örn: "Admin", "User", "Moderator")
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Normalize edilmiş rol adı (case-insensitive arama için)
    /// </summary>
    public required string NormalizedName { get; set; }

    /// <summary>
    /// Rol açıklaması
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Concurrency stamp - optimistic concurrency için
    /// </summary>
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Bu role atanmış permission'lar
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    /// <summary>
    /// Bu role sahip kullanıcılar
    /// </summary>
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
