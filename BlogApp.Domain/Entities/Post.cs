using BlogApp.Domain.Common;
using System.Collections.Generic;

namespace BlogApp.Domain.Entities;

public class Post : BaseEntity
{
    public Post()
    {

    }
    public Post(int categoryId, string title, string body, string summary)
    {
        Title = title;
        Body = body;
        Summary = summary;
        CategoryId = categoryId;
    }

    public string Title { get; set; }
    public string Body { get; set; }
    public string Summary { get; set; }
    public string Thumbnail { get; set; }
    public bool IsPublished { get; set; }
    public virtual ICollection<Comment>? Comments { get; set; }
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; }

}
