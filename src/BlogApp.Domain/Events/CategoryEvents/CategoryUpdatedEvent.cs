using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.CategoryEvents;

[StoreInOutbox]
public class CategoryUpdatedEvent : DomainEvent
{
    public int CategoryId { get; }
    public string Name { get; }
    public int UpdatedById { get; }

    public CategoryUpdatedEvent(int categoryId, string name, int updatedById)
    {
        CategoryId = categoryId;
        Name = name;
        UpdatedById = updatedById;
    }
}
