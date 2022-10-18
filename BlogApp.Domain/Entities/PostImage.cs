using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities
{
    public class PostImage : BaseEntity
    {
        public virtual Post? Post { get; set; }
        public int PostId { get; set; }
        public virtual Image? Image { get; set; }
        public int ImageId { get; set; }
    }
}
