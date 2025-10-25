using BlogApp.Domain.Common;

namespace BlogApp.Domain.Events.UserEvents;

/// <summary>
/// Domain event raised when a new user is created
/// </summary>
public class UserCreatedEvent : DomainEvent
{
    public int UserId { get; }
    public string UserName { get; }
    public string Email { get; }
    public int? CreatedById { get; }

    public UserCreatedEvent(int userId, string userName, string email, int? createdById)
    {
        UserId = userId;
        UserName = userName;
        Email = email;
        CreatedById = createdById;
    }
}
