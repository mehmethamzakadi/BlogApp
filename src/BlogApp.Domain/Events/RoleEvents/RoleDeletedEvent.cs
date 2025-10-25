using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.RoleEvents;

/// <summary>
/// Bir rol silindiğinde tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class RoleDeletedEvent : DomainEvent
{
    public int RoleId { get; }
    public string RoleName { get; }
    public int? DeletedById { get; }

    public RoleDeletedEvent(int roleId, string roleName, int? deletedById)
    {
        RoleId = roleId;
        RoleName = roleName;
        DeletedById = deletedById;
    }
}
