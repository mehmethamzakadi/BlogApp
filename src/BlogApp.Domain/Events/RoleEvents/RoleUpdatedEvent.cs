using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.RoleEvents;

/// <summary>
/// Bir rol güncellendiğinde tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class RoleUpdatedEvent : DomainEvent
{
    public int RoleId { get; }
    public string RoleName { get; }
    public int? UpdatedById { get; }

    public RoleUpdatedEvent(int roleId, string roleName, int? updatedById)
    {
        RoleId = roleId;
        RoleName = roleName;
        UpdatedById = updatedById;
    }
}
