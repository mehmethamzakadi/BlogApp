using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.PostEvents;

/// <summary>
/// Bir gönderi silindiğinde tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class PostDeletedEvent : DomainEvent
{
    public int PostId { get; }
    public string Title { get; }
    public int DeletedById { get; }

    public PostDeletedEvent(int postId, string title, int deletedById)
    {
        PostId = postId;
        Title = title;
        DeletedById = deletedById;
    }
}
