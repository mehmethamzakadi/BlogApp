using BlogApp.Domain.Common;

namespace BlogApp.Domain.Events;

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
