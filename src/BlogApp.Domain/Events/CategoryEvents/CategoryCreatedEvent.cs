using BlogApp.Domain.Common;

namespace BlogApp.Domain.Events.CategoryEvents;

/// <summary>
/// Domain event raised when a category is created
/// </summary>
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
