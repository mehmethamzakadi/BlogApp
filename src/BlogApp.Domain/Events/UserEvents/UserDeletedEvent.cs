using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.UserEvents;

/// <summary>
/// Bir kullanıcı silindiğinde tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class UserDeletedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string UserName { get; }
    public string Email { get; }
    public Guid? DeletedById { get; }

    public UserDeletedEvent(Guid userId, string userName, string email, Guid? deletedById)
    {
        UserId = userId;
        UserName = userName;
        Email = email;
        DeletedById = deletedById;
    }
}
