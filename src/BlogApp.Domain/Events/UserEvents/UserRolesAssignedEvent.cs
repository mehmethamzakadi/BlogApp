using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.UserEvents;

/// <summary>
/// Bir kullanıcıya roller atandığında tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class UserRolesAssignedEvent : DomainEvent
{
    public int UserId { get; }
    public string UserName { get; }
    public IReadOnlyList<string> RoleNames { get; }
    public int? AssignedById { get; }

    public UserRolesAssignedEvent(int userId, string userName, IReadOnlyList<string> roleNames, int? assignedById)
    {
        UserId = userId;
        UserName = userName;
        RoleNames = roleNames;
        AssignedById = assignedById;
    }
}
