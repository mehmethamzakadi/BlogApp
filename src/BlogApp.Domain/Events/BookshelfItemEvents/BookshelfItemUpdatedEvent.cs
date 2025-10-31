using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.BookshelfItemEvents;

[StoreInOutbox]
public sealed class BookshelfItemUpdatedEvent : DomainEvent
{
    public Guid ItemId { get; }
    public string Title { get; }
    public override Guid AggregateId => ItemId;

    public BookshelfItemUpdatedEvent(Guid itemId, string title)
    {
        ItemId = itemId;
        Title = title;
    }
}