using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.RoleEvents;

/// <summary>
/// Yeni bir rol oluşturulduğunda tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class RoleCreatedEvent : DomainEvent
{
    public Guid RoleId { get; }
    public string RoleName { get; }
    public Guid? CreatedById { get; }

    public RoleCreatedEvent(Guid roleId, string roleName, Guid? createdById)
    {
        RoleId = roleId;
        RoleName = roleName;
        CreatedById = createdById;
    }
}
