using BlogApp.Domain.Common;
using BlogApp.Domain.Events.PostEvents;

namespace BlogApp.Domain.Entities;

/// <summary>
/// Post aggregate root
/// ✅ DDD: Comments removed - Comment is now a separate aggregate
/// </summary>
public sealed class Post : AggregateRoot
{
    // EF Core için parameterless constructor
    public Post() { }

    public string Title { get; private set; } = default!;
    public string Body { get; private set; } = default!;
    public string Summary { get; private set; } = default!;
    public string Thumbnail { get; private set; } = string.Empty;
    public bool IsPublished { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = default!;

    public static Post Create(string title, string body, string summary, Guid categoryId, string? thumbnail = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new Exceptions.DomainValidationException("Title cannot be empty");
        if (string.IsNullOrWhiteSpace(body))
            throw new Exceptions.DomainValidationException("Body cannot be empty");
        if (categoryId == Guid.Empty)
            throw new Exceptions.DomainValidationException("CategoryId is required");

        var post = new Post
        {
            Title = title,
            Body = body,
            Summary = summary,
            CategoryId = categoryId,
            Thumbnail = thumbnail ?? string.Empty,
            IsPublished = false
        };

        post.AddDomainEvent(new PostCreatedEvent(post.Id, title, categoryId));
        return post;
    }

    public void Update(string title, string body, string summary, Guid categoryId, string? thumbnail = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new Exceptions.DomainValidationException("Title cannot be empty");
        if (string.IsNullOrWhiteSpace(body))
            throw new Exceptions.DomainValidationException("Body cannot be empty");

        Title = title;
        Body = body;
        Summary = summary;
        CategoryId = categoryId;
        if (thumbnail != null)
            Thumbnail = thumbnail;

        AddDomainEvent(new PostUpdatedEvent(Id, title));
    }

    public void Publish()
    {
        if (IsPublished)
            throw new InvalidOperationException("Post is already published");

        IsPublished = true;
    }

    public void Unpublish()
    {
        if (!IsPublished)
            throw new InvalidOperationException("Post is not published");

        IsPublished = false;
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Post is already deleted");

        AddDomainEvent(new PostDeletedEvent(Id, Title));
    }
}