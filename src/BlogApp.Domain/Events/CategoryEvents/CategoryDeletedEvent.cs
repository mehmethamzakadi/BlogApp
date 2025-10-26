using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.CategoryEvents;

[StoreInOutbox]
public class CategoryDeletedEvent : DomainEvent
{
    public Guid CategoryId { get; }
    public string Name { get; }
    public Guid DeletedById { get; }

    public CategoryDeletedEvent(Guid categoryId, string name, Guid deletedById)
    {
        CategoryId = categoryId;
        Name = name;
        DeletedById = deletedById;
    }
}
