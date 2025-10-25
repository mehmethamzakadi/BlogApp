using BlogApp.Domain.Common;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Domain.Entities;

public sealed class AppUser : IdentityUser<int>, IHasDomainEvents
{
    // Domain Events implementation
    private readonly List<IDomainEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
