using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.RoleEvents;

/// <summary>
/// Bir rol silindiğinde tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class RoleDeletedEvent : DomainEvent
{
    public Guid RoleId { get; }
    public string RoleName { get; }
    public Guid? DeletedById { get; }

    public RoleDeletedEvent(Guid roleId, string roleName, Guid? deletedById)
    {
        RoleId = roleId;
        RoleName = roleName;
        DeletedById = deletedById;
    }
}
