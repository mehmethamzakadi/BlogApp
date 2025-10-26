using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.BookshelfItemEvents;

[StoreInOutbox]
public sealed class BookshelfItemCreatedEvent : DomainEvent
{
	public Guid ItemId { get; }
	public string Title { get; }
	public Guid ActorId { get; }

	public BookshelfItemCreatedEvent(Guid itemId, string title, Guid actorId)
	{
		ItemId = itemId;
		Title = title;
		ActorId = actorId;
	}
}
