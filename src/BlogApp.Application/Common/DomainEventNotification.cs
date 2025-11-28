using BlogApp.Domain.Common;
using MediatR;

namespace BlogApp.Application.Common;

/// <summary>
/// Domain event'leri MediatR notification'larına dönüştüren wrapper.
/// Bu sayede Domain katmanı MediatR'dan bağımsız kalır.
/// </summary>
/// <typeparam name="TDomainEvent">Domain event tipi</typeparam>
public sealed class DomainEventNotification<TDomainEvent> : INotification
    where TDomainEvent : IDomainEvent
{
    public TDomainEvent DomainEvent { get; }

    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent ?? throw new ArgumentNullException(nameof(domainEvent));
    }
}
