namespace BlogApp.Domain.Common;

/// <summary>
/// Domain event'ler için temel sınıf
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
