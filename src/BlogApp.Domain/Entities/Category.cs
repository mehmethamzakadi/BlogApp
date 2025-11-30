using BlogApp.Domain.Common;
using BlogApp.Domain.Events.CategoryEvents;

namespace BlogApp.Domain.Entities;

public sealed class Category : AggregateRoot
{
    // EF Core ve seed'ler için parameterless constructor
    public Category() { }

    public string Name { get; private set; } = default!;
    public string? NormalizedName { get; private set; }
    public string? Description { get; private set; }
    public Guid? ParentId { get; private set; }
    
    // Navigation properties
    public Category? Parent { get; private set; }
    public ICollection<Category> Children { get; private set; } = new List<Category>();
    public ICollection<Post> Posts { get; private set; } = new List<Post>();

    public static Category Create(string name, string? description = null, Guid? parentId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exceptions.DomainValidationException("Category name cannot be empty");

        var category = new Category
        {
            Name = name,
            NormalizedName = name.ToUpperInvariant(),
            Description = description,
            ParentId = parentId
        };

        category.AddDomainEvent(new CategoryCreatedEvent(category.Id, name));
        return category;
    }

    public void Update(string name, string? description = null, Guid? parentId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exceptions.DomainValidationException("Category name cannot be empty");

        // Kendi kendisinin parent'ı olamaz
        if (parentId.HasValue && parentId.Value == Id)
            throw new Exceptions.DomainValidationException("Category cannot be its own parent");

        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
        ParentId = parentId;

        AddDomainEvent(new CategoryUpdatedEvent(Id, name));
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Category is already deleted");

        AddDomainEvent(new CategoryDeletedEvent(Id, Name));
    }
}