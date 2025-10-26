using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.RoleEvents;

/// <summary>
/// Bir rol güncellendiğinde tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class RoleUpdatedEvent : DomainEvent
{
    public Guid RoleId { get; }
    public string RoleName { get; }
    public Guid? UpdatedById { get; }

    public RoleUpdatedEvent(Guid roleId, string roleName, Guid? updatedById)
    {
        RoleId = roleId;
        RoleName = roleName;
        UpdatedById = updatedById;
    }
}
