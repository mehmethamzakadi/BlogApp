using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

public sealed class Comment : BaseEntity
{
    public Comment() { }

    public Guid? ParentId { get; set; }
    public Comment? Parent { get; set; }
    public Guid PostId { get; set; }
    public Post? Post { get; set; }
    public string Content { get; set; } = default!;
    public string CommentOwnerMail { get; set; } = default!;
    public bool IsPublished { get; set; }
}
