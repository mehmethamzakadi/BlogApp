using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.PostEvents;

[StoreInOutbox]
public class PostDeletedEvent : DomainEvent
{
    public Guid PostId { get; }
    public string Title { get; }
    public override Guid AggregateId => PostId;

    public PostDeletedEvent(Guid postId, string title)
    {
        PostId = postId;
        Title = title;
    }
}