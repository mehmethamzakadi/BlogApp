using BlogApp.Domain.Common;

namespace BlogApp.Domain.Events.UserEvents;

/// <summary>
/// Domain event raised when roles are assigned to a user
/// </summary>
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
