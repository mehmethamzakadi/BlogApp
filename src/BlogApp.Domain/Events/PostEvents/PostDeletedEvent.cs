using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.PostEvents;

/// <summary>
/// Bir gönderi silindiğinde tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class PostDeletedEvent : DomainEvent
{
    public Guid PostId { get; }
    public string Title { get; }
    public Guid DeletedById { get; }

    public PostDeletedEvent(Guid postId, string title, Guid deletedById)
    {
        PostId = postId;
        Title = title;
        DeletedById = deletedById;
    }
}
