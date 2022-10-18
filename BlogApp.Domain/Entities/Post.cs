using BlogApp.Domain.Common;
using System.Collections.Generic;

namespace BlogApp.Domain.Entities
{
    public class Post : BaseEntity
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Summary { get; set; }
        public string Thumbnail { get; set; }
        public bool IsPublished { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
    }
}
