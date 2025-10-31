using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;

namespace BlogApp.Domain.Events.CategoryEvents;

[StoreInOutbox]
public class CategoryUpdatedEvent : DomainEvent
{
    public Guid CategoryId { get; }
    public string Name { get; }
    public override Guid AggregateId => CategoryId;

    public CategoryUpdatedEvent(Guid categoryId, string name)
    {
        CategoryId = categoryId;
        Name = name;
    }
}