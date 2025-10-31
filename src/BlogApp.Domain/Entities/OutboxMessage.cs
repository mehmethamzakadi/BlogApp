using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

/// <summary>
/// Güvenilir mesaj iletimi için Outbox pattern uygulaması
/// Domain event'leri message broker'a yayınlanmadan önce saklar
/// </summary>
public class OutboxMessage : BaseEntity
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public int RetryCount { get; set; } = 0;
    public string? Error { get; set; }
    public DateTime? NextRetryAt { get; set; }
}
