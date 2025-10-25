using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

public class ActivityLog : BaseEntity
{
    public string ActivityType { get; set; } = string.Empty; // post_created, post_updated, post_deleted, category_created, vb.
    public string EntityType { get; set; } = string.Empty;   // Post, Category, User, vb.
    public int? EntityId { get; set; }                       // Etkilenen entity'nin ID'si
    public string Title { get; set; } = string.Empty;        // İnsan tarafından okunabilir açıklama
    public string? Details { get; set; }                     // JSON veya ek bilgi
    public int? UserId { get; set; }                         // İşlemi gerçekleştiren kullanıcı
    public User? User { get; set; }
    public System.DateTime Timestamp { get; set; } = System.DateTime.UtcNow;
}
