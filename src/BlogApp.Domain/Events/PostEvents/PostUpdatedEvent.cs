using BlogApp.Domain.Common;

namespace BlogApp.Domain.Events.PostEvents;

/// <summary>
/// Domain event raised when a post is updated
/// </summary>
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
