using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.UserEvents;

[StoreInOutbox]
public class UserDeletedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string UserName { get; }

    public UserDeletedEvent(Guid userId, string userName)
    {
        UserId = userId;
        UserName = userName;
    }
}