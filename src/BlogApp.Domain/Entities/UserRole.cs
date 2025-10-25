namespace BlogApp.Domain.Entities;

/// <summary>
/// Kullanıcı ve Rol arasındaki many-to-many ilişkiyi temsil eder.
/// Bir kullanıcının hangi rollere sahip olduğunu tanımlar.
/// </summary>
public sealed class UserRole
{
    /// <summary>
    /// Kullanıcı ID'si
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Navigation property: İlişkili kullanıcı
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Rol ID'si
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Navigation property: İlişkili rol
    /// </summary>
    public Role Role { get; set; } = null!;

    /// <summary>
    /// Rolün kullanıcıya atandığı tarih
    /// </summary>
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
}
