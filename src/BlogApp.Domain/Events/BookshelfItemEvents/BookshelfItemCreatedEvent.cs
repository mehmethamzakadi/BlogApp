using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.BookshelfItemEvents;

[StoreInOutbox]
public sealed class BookshelfItemCreatedEvent : DomainEvent
{
    public Guid ItemId { get; }
    public string Title { get; }
    public override Guid AggregateId => ItemId;

    public BookshelfItemCreatedEvent(Guid itemId, string title)
    {
        ItemId = itemId;
        Title = title;
    }
}