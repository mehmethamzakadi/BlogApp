namespace BlogApp.Domain.Entities;

/// <summary>
/// Rol ve Permission arasındaki many-to-many ilişkiyi temsil eder.
/// Bir rolün hangi permission'lara sahip olduğunu tanımlar.
/// </summary>
public sealed class AppRolePermission
{
    /// <summary>
    /// Rol ID'si
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Navigation property: İlişkili rol
    /// </summary>
    public AppRole Role { get; set; } = null!;

    /// <summary>
    /// Permission ID'si
    /// </summary>
    public int PermissionId { get; set; }

    /// <summary>
    /// Navigation property: İlişkili permission
    /// </summary>
    public Permission Permission { get; set; } = null!;

    /// <summary>
    /// Permission'ın bu role atandığı tarih
    /// </summary>
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
}
