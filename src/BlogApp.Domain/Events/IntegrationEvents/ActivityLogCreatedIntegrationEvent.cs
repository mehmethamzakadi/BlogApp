namespace BlogApp.Domain.Events.IntegrationEvents;

/// <summary>
/// Integration event for creating activity log via RabbitMQ
/// This event is published to message broker for async processing
/// </summary>
public record ActivityLogCreatedIntegrationEvent(
    string ActivityType,
    string EntityType,
    int? EntityId,
    string Title,
    string? Details,
    int? UserId,
    DateTime Timestamp
);
