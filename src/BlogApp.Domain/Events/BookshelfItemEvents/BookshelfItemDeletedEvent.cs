using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.BookshelfItemEvents;

[StoreInOutbox]
public sealed class BookshelfItemDeletedEvent : DomainEvent
{
	public Guid ItemId { get; }
	public string Title { get; }
	public Guid ActorId { get; }

	public BookshelfItemDeletedEvent(Guid itemId, string title, Guid actorId)
	{
		ItemId = itemId;
		Title = title;
		ActorId = actorId;
	}
}
