namespace BlogApp.Domain.Events.IntegrationEvents;

/// <summary>
/// RabbitMQ üzerinden aktivite logu oluşturmak için integration event
/// Bu event, asenkron işleme için message broker'a yayınlanır
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
