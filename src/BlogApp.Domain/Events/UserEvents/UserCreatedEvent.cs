using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.UserEvents;

/// <summary>
/// Yeni bir kullanıcı oluşturulduğunda tetiklenen domain event
/// </summary>
[StoreInOutbox]
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
