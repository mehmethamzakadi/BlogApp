using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

public class ActivityLog : BaseEntity
{
    public string ActivityType { get; set; } = string.Empty; // post_created, post_updated, post_deleted, category_created, etc.
    public string EntityType { get; set; } = string.Empty;   // Post, Category, User, etc.
    public int? EntityId { get; set; }                       // ID of the affected entity
    public string Title { get; set; } = string.Empty;        // Human-readable description
    public string? Details { get; set; }                     // JSON or additional info
    public int? UserId { get; set; }                         // Who performed the action
    public AppUser? User { get; set; }
    public System.DateTime Timestamp { get; set; } = System.DateTime.UtcNow;
}
