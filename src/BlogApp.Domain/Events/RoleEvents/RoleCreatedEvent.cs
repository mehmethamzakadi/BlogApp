using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.RoleEvents;

[StoreInOutbox]
public class RoleCreatedEvent : DomainEvent
{
    public Guid RoleId { get; }
    public string RoleName { get; }

    public RoleCreatedEvent(Guid roleId, string roleName)
    {
        RoleId = roleId;
        RoleName = roleName;
    }
}