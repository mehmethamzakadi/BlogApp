using BlogApp.Domain.Common;
using BlogApp.Domain.Events.PostEvents;
using System.Linq;

namespace BlogApp.Domain.Entities;

public sealed class Post : AggregateRoot
{
    private readonly List<Comment> _comments = new();

    public Post() { }

    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public string Thumbnail { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!;

    public static Post Create(string title, string body, string summary, Guid categoryId, string? thumbnail = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Body cannot be empty", nameof(body));
        if (categoryId == Guid.Empty)
            throw new ArgumentException("CategoryId is required", nameof(categoryId));

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
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Body cannot be empty", nameof(body));

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

    public void AddComment(string content, string ownerEmail, Guid? parentId = null)
    {
        var comment = Comment.Create(Id, content, ownerEmail, parentId);
        _comments.Add(comment);
    }

    public void RemoveComment(Guid commentId)
    {
        var comment = _comments.FirstOrDefault(c => c.Id == commentId);
        if (comment == null)
            throw new InvalidOperationException($"Comment {commentId} not found");

        _comments.Remove(comment);
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Post is already deleted");

        AddDomainEvent(new PostDeletedEvent(Id, Title));
    }
}