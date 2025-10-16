
using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations
{
    public class CommentConfiguration : BaseConfiguraiton<Comment>
    {
        public override void Configure(EntityTypeBuilder<Comment> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Content).IsRequired().HasMaxLength(300);
            builder.Property(x => x.CommentOwnerMail).IsRequired().HasMaxLength(100);
        }
    }
}
