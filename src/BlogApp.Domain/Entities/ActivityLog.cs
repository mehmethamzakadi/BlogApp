namespace BlogApp.Domain.Entities;

/// <summary>
/// Activity Log - Kullanıcı aktivitelerini takip eder
/// BaseEntity'den türemez çünkü auditing'e ihtiyacı yok
/// </summary>
public class ActivityLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ActivityType { get; set; } = string.Empty; // post_created, post_updated, post_deleted
    public string EntityType { get; set; } = string.Empty;   // Post, Category, User
    public Guid? EntityId { get; set; }                      // Etkilenen entity'nin ID'si
    public string Title { get; set; } = string.Empty;        // İnsan tarafından okunabilir açıklama
    public string? Details { get; set; }                     // JSON veya ek bilgi
    public Guid UserId { get; set; }                         // İşlemi gerçekleştiren kullanıcı
    public User? User { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
