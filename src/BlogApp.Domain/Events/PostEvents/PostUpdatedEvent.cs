using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.PostEvents;

/// <summary>
/// Bir gönderi güncellendiğinde tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class PostUpdatedEvent : DomainEvent
{
    public Guid PostId { get; }
    public string Title { get; }
    public Guid UpdatedById { get; }

    public PostUpdatedEvent(Guid postId, string title, Guid updatedById)
    {
        PostId = postId;
        Title = title;
        UpdatedById = updatedById;
    }
}
