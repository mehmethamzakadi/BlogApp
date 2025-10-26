using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.UserEvents;

/// <summary>
/// Bir kullanıcıya roller atandığında tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class UserRolesAssignedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string UserName { get; }
    public IReadOnlyList<string> RoleNames { get; }
    public Guid? AssignedById { get; }

    public UserRolesAssignedEvent(Guid userId, string userName, IReadOnlyList<string> roleNames, Guid? assignedById)
    {
        UserId = userId;
        UserName = userName;
        RoleNames = roleNames;
        AssignedById = assignedById;
    }
}
