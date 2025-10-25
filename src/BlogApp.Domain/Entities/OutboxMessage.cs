using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

/// <summary>
/// Outbox pattern implementation for reliable message delivery
/// Stores domain events before publishing to message broker
/// </summary>
public class OutboxMessage : BaseEntity
{
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public int RetryCount { get; set; } = 0;
    public string? Error { get; set; }
    public DateTime? NextRetryAt { get; set; }
}
