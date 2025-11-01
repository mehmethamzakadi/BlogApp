namespace BlogApp.Domain.Common;

/// <summary>
/// Domain event'ler için temel sınıf
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public abstract Guid AggregateId { get; }
}
