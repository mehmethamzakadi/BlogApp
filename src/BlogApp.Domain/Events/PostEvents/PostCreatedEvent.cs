using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.PostEvents;

/// <summary>
/// Bir gönderi oluşturulduğunda tetiklenen domain event
/// </summary>
[StoreInOutbox]
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
