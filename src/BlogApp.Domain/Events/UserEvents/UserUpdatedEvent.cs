using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.UserEvents;

/// <summary>
/// Bir kullanıcı güncellendiğinde tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class UserUpdatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string UserName { get; }
    public string Email { get; }
    public Guid? UpdatedById { get; }

    public UserUpdatedEvent(Guid userId, string userName, string email, Guid? updatedById)
    {
        UserId = userId;
        UserName = userName;
        Email = email;
        UpdatedById = updatedById;
    }
}
