using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

/// <summary>
/// Sistemdeki kullanıcıları temsil eder.
/// Identity'den bağımsız, custom user entity.
/// </summary>
public sealed class User : BaseEntity
{
    private string _userName = default!;
    private string _email = default!;

    /// <summary>
    /// Kullanıcı adı (benzersiz olmalı)
    /// </summary>
    public string UserName
    {
        get => _userName;
        private set => _userName = value;
    }

    /// <summary>
    /// Normalize edilmiş kullanıcı adı (case-insensitive arama için)
    /// </summary>
    public string NormalizedUserName { get; private set; } = string.Empty;

    /// <summary>
    /// Email adresi (benzersiz olmalı)
    /// </summary>
    public string Email
    {
        get => _email;
        private set => _email = value;
    }

    /// <summary>
    /// Normalize edilmiş email (case-insensitive arama için)
    /// </summary>
    public string NormalizedEmail { get; private set; } = string.Empty;

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

    public static User Create(string userName, string email, string passwordHash)
    {
        var userNameVO = ValueObjects.UserName.Create(userName);
        var emailVO = ValueObjects.Email.Create(email);

        var user = new User
        {
            UserName = userNameVO.Value,
            NormalizedUserName = userNameVO.Value.ToUpperInvariant(),
            Email = emailVO.Value,
            NormalizedEmail = emailVO.Value.ToUpperInvariant(),
            PasswordHash = passwordHash,
            EmailConfirmed = false
        };

        user.AddDomainEvent(new Domain.Events.UserEvents.UserCreatedEvent(user.Id, userName, email));
        return user;
    }

    public void Update(string userName, string email)
    {
        var userNameVO = ValueObjects.UserName.Create(userName);
        var emailVO = ValueObjects.Email.Create(email);

        UserName = userNameVO.Value;
        NormalizedUserName = userNameVO.Value.ToUpperInvariant();
        Email = emailVO.Value;
        NormalizedEmail = emailVO.Value.ToUpperInvariant();

        AddDomainEvent(new Domain.Events.UserEvents.UserUpdatedEvent(Id, userName));
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("User is already deleted");

        AddDomainEvent(new Domain.Events.UserEvents.UserDeletedEvent(Id, UserName));
    }
}