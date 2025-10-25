using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.UserEvents;

/// <summary>
/// Bir kullanıcı silindiğinde tetiklenen domain event
/// </summary>
[StoreInOutbox]
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
