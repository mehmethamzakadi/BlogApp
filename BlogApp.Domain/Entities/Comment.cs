using BlogApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApp.Domain.Entities
{
    public class Comment: BaseEntity
    {
        public int? ParentId { get; set; }
        public virtual Comment Parent { get; set; }
        public int PostId { get; set; }
        public virtual Post Post { get; set; }
        public string Content { get; set; }
        public string CommentOwnerMail { get; set; }
        public bool IsPublished { get; set; }
    }
}
