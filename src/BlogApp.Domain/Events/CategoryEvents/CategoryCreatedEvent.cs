using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.CategoryEvents;

/// <summary>
/// Bir kategori oluşturulduğunda tetiklenen domain event
/// </summary>
[StoreInOutbox]
public class CategoryCreatedEvent : DomainEvent
{
    public Guid CategoryId { get; }
    public string Name { get; }
    public Guid CreatedById { get; }

    public CategoryCreatedEvent(Guid categoryId, string name, Guid createdById)
    {
        CategoryId = categoryId;
        Name = name;
        CreatedById = createdById;
    }
}
