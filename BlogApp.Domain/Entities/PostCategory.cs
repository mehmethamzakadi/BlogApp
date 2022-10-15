using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities
{
    public class PostCategory : IEntity
    {
        public int Id { get; set; }
        public virtual Category Category { get; set; }
        public int CategoryId { get; set; }
        public virtual Post Post { get; set; }
        public int PostId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
