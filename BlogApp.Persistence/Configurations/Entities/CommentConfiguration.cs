using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations.Entities
{
    public class CommentConfiguration : BaseConfiguraiton<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.Property(x => x.Content).IsRequired().HasMaxLength(300);
            builder.Property(x => x.CommentOwnerMail).IsRequired().HasMaxLength(100);
        }
    }
}
