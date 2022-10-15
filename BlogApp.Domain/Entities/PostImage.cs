using BlogApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApp.Domain.Entities
{
    public class PostImage : IEntity
    {
        public int Id { get; set; }
        public virtual Post Post { get; set; }
        public int PostId { get; set; }
        public virtual Image Image { get; set; }
        public int ImageId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
