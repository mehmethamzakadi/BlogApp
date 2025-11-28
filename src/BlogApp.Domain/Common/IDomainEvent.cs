namespace BlogApp.Domain.Common;

/// <summary>
/// Domain event'ler için marker interface.
/// Domain event'ler, domain uzmanlarının önemsediği domain'de gerçekleşen bir şeyi temsil eder.
/// 
/// NOT: Bu interface MediatR'dan bağımsızdır. Application katmanında MediatR entegrasyonu
/// için IDomainEventNotification wrapper'ı kullanılır.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Event'in benzersiz kimliği
    /// </summary>
    Guid EventId { get; }
    
    /// <summary>
    /// Event'in oluşturulma zamanı (UTC)
    /// </summary>
    DateTime OccurredOn { get; }
    
    /// <summary>
    /// Event'i oluşturan aggregate'in ID'si
    /// </summary>
    Guid AggregateId { get; }
}
