using BlogApp.Domain.Common;

namespace BlogApp.Domain.Events.PostEvents;

/// <summary>
/// Domain event raised when a post is deleted
/// </summary>
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
