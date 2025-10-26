using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.UserEvents;

/// <summary>
/// Yeni bir kullanıcı oluşturulduğunda tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class UserCreatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string UserName { get; }
    public string Email { get; }
    public Guid? CreatedById { get; }

    public UserCreatedEvent(Guid userId, string userName, string email, Guid? createdById)
    {
        UserId = userId;
        UserName = userName;
        Email = email;
        CreatedById = createdById;
    }
}
