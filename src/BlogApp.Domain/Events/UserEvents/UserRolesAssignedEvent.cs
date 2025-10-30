using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.UserEvents;

[StoreInOutbox]
public class UserRolesAssignedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string UserName { get; }
    public IReadOnlyList<string> RoleNames { get; }

    public UserRolesAssignedEvent(Guid userId, string userName, IReadOnlyList<string> roleNames)
    {
        UserId = userId;
        UserName = userName;
        RoleNames = roleNames;
    }
}