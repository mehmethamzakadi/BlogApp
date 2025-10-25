using BlogApp.Domain.Common;

namespace BlogApp.Domain.Events.CategoryEvents;

public class CategoryDeletedEvent : DomainEvent
{
    public int CategoryId { get; }
    public string Name { get; }
    public int DeletedById { get; }

    public CategoryDeletedEvent(int categoryId, string name, int deletedById)
    {
        CategoryId = categoryId;
        Name = name;
        DeletedById = deletedById;
    }
}
