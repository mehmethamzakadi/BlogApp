using BlogApp.Domain.Common;

namespace BlogApp.Domain.Events.UserEvents;

/// <summary>
/// Domain event raised when a user is deleted
/// </summary>
public class UserDeletedEvent : DomainEvent
{
    public int UserId { get; }
    public string UserName { get; }
    public string Email { get; }
    public int? DeletedById { get; }

    public UserDeletedEvent(int userId, string userName, string email, int? deletedById)
    {
        UserId = userId;
        UserName = userName;
        Email = email;
        DeletedById = deletedById;
    }
}
