using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.PostEvents;

[StoreInOutbox]
public class PostCreatedEvent : DomainEvent
{
    public Guid PostId { get; }
    public string Title { get; }
    public Guid CategoryId { get; }
    public override Guid AggregateId => PostId;

    public PostCreatedEvent(Guid postId, string title, Guid categoryId)
    {
        PostId = postId;
        Title = title;
        CategoryId = categoryId;
    }
}