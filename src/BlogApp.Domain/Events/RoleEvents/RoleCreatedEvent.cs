using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.RoleEvents;

/// <summary>
/// Yeni bir rol oluşturulduğunda tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class RoleCreatedEvent : DomainEvent
{
    public int RoleId { get; }
    public string RoleName { get; }
    public int? CreatedById { get; }

    public RoleCreatedEvent(int roleId, string roleName, int? createdById)
    {
        RoleId = roleId;
        RoleName = roleName;
        CreatedById = createdById;
    }
}
