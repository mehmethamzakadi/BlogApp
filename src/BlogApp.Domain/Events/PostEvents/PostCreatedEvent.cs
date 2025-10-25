using BlogApp.Domain.Common;

namespace BlogApp.Domain.Events.PostEvents;

/// <summary>
/// Domain event raised when a post is created
/// </summary>
public class PostCreatedEvent : DomainEvent
{
    public int PostId { get; }
    public string Title { get; }
    public int CategoryId { get; }
    public int CreatedById { get; }

    public PostCreatedEvent(int postId, string title, int categoryId, int createdById)
    {
        PostId = postId;
        Title = title;
        CategoryId = categoryId;
        CreatedById = createdById;
    }
}
