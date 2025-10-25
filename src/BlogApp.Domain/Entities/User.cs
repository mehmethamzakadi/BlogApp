using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

/// <summary>
/// Sistemdeki kullanıcıları temsil eder.
/// Identity'den bağımsız, custom user entity.
/// </summary>
public sealed class User : BaseEntity
{
    /// <summary>
    /// Kullanıcı adı (benzersiz olmalı)
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// Normalize edilmiş kullanıcı adı (case-insensitive arama için)
    /// </summary>
    public required string NormalizedUserName { get; set; }

    /// <summary>
    /// Email adresi (benzersiz olmalı)
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Normalize edilmiş email (case-insensitive arama için)
    /// </summary>
    public required string NormalizedEmail { get; set; }

    /// <summary>
    /// Email adresinin doğrulanıp doğrulanmadığı
    /// </summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>
    /// Hashed password (PBKDF2 ile hash'lenmiş)
    /// </summary>
    public required string PasswordHash { get; set; }

    /// <summary>
    /// Security stamp - parola değiştiğinde güncellenir
    /// Eski token'ları geçersiz kılmak için kullanılır
    /// </summary>
    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Concurrency stamp - optimistic concurrency için
    /// </summary>
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Telefon numarası
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Telefon numarasının doğrulanıp doğrulanmadığı
    /// </summary>
    public bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// İki faktörlü doğrulama aktif mi?
    /// </summary>
    public bool TwoFactorEnabled { get; set; }

    /// <summary>
    /// Hesap kilitlenme bitiş zamanı (null ise kilitli değil)
    /// </summary>
    public DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// Hesap kilitlenme özelliği aktif mi?
    /// </summary>
    public bool LockoutEnabled { get; set; } = true;

    /// <summary>
    /// Başarısız giriş denemesi sayısı
    /// </summary>
    public int AccessFailedCount { get; set; }

    /// <summary>
    /// Aktif refresh token
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Refresh token son kullanma tarihi
    /// </summary>
    public DateTime? RefreshTokenExpiry { get; set; }

    /// <summary>
    /// Şifre sıfırlama token'ı
    /// </summary>
    public string? PasswordResetToken { get; set; }

    /// <summary>
    /// Şifre sıfırlama token'ının son kullanma tarihi
    /// </summary>
    public DateTime? PasswordResetTokenExpiry { get; set; }

    /// <summary>
    /// Kullanıcının rolleri
    /// </summary>
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    /// <summary>
    /// Kullanıcının hesabının kilitli olup olmadığını kontrol eder
    /// </summary>
    public bool IsLockedOut()
    {
        return LockoutEnabled && LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow;
    }
}
