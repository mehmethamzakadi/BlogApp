using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.PostEvents;

[StoreInOutbox]
public class PostUpdatedEvent : DomainEvent
{
    public Guid PostId { get; }
    public string Title { get; }
    public override Guid AggregateId => PostId;

    public PostUpdatedEvent(Guid postId, string title)
    {
        PostId = postId;
        Title = title;
    }
}