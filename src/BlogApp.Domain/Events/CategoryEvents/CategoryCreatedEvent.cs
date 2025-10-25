using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.CategoryEvents;

/// <summary>
/// Bir kategori oluşturulduğunda tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class CategoryCreatedEvent : DomainEvent
{
    public int CategoryId { get; }
    public string Name { get; }
    public int CreatedById { get; }

    public CategoryCreatedEvent(int categoryId, string name, int createdById)
    {
        CategoryId = categoryId;
        Name = name;
        CreatedById = createdById;
    }
}
