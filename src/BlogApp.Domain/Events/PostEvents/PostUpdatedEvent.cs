using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.PostEvents;

/// <summary>
/// Bir gönderi güncellendiğinde tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class PostUpdatedEvent : DomainEvent
{
    public int PostId { get; }
    public string Title { get; }
    public int UpdatedById { get; }

    public PostUpdatedEvent(int postId, string title, int updatedById)
    {
        PostId = postId;
        Title = title;
        UpdatedById = updatedById;
    }
}
