using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Domain.Common;

public abstract class BaseEntity : IEntityTimestamps
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CreatedById { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public int? UpdatedById { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    // Domain Events
    private readonly List<IDomainEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
