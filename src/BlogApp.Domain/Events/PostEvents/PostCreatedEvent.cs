using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.PostEvents;

/// <summary>
/// Bir gönderi oluşturulduğunda tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class PostCreatedEvent : DomainEvent
{
    public Guid PostId { get; }
    public string Title { get; }
    public Guid CategoryId { get; }
    public Guid CreatedById { get; }

    public PostCreatedEvent(Guid postId, string title, Guid categoryId, Guid createdById)
    {
        PostId = postId;
        Title = title;
        CategoryId = categoryId;
        CreatedById = createdById;
    }
}
