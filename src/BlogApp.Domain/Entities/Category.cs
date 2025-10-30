using BlogApp.Domain.Common;
using BlogApp.Domain.Events.CategoryEvents;

namespace BlogApp.Domain.Entities;

public sealed class Category : BaseEntity
{
    public Category() { } // EF Core ve seed'ler için

    public string Name { get; set; } = default!;
    public string? NormalizedName { get; set; }

    public static Category Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        var category = new Category
        {
            Name = name,
            NormalizedName = name.ToUpperInvariant()
        };

        category.AddDomainEvent(new CategoryCreatedEvent(category.Id, name));
        return category;
    }

    public void Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        Name = name;
        NormalizedName = name.ToUpperInvariant();

        AddDomainEvent(new CategoryUpdatedEvent(Id, name));
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Category is already deleted");

        AddDomainEvent(new CategoryDeletedEvent(Id, Name));
    }
}