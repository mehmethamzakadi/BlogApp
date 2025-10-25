using BlogApp.Domain.Common;

namespace BlogApp.Domain.Events.UserEvents;

/// <summary>
/// Domain event raised when a user is updated
/// </summary>
public class UserUpdatedEvent : DomainEvent
{
    public int UserId { get; }
    public string UserName { get; }
    public string Email { get; }
    public int? UpdatedById { get; }

    public UserUpdatedEvent(int userId, string userName, string email, int? updatedById)
    {
        UserId = userId;
        UserName = userName;
        Email = email;
        UpdatedById = updatedById;
    }
}
