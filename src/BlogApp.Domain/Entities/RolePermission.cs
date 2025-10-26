namespace BlogApp.Domain.Entities;

/// <summary>
/// Rol ve Permission arasındaki many-to-many ilişkiyi temsil eder.
/// Bir rolün hangi permission'lara sahip olduğunu tanımlar.
/// </summary>
public sealed class RolePermission
{
    /// <summary>
    /// Rol ID'si
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Navigation property: İlişkili rol
    /// </summary>
    public Role Role { get; set; } = null!;

    /// <summary>
    /// Permission ID'si
    /// </summary>
    public Guid PermissionId { get; set; }

    /// <summary>
    /// Navigation property: İlişkili permission
    /// </summary>
    public Permission Permission { get; set; } = null!;

    /// <summary>
    /// Permission'ın bu role atandığı tarih
    /// </summary>
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
}
