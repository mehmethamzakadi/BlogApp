using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations.Entities
{
    public class PostConfiguration : BaseConfiguraiton<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Property(x => x.Body).IsRequired();
            builder.Property(x => x.Summary).IsRequired();
            builder.Property(x => x.Thumbnail).IsRequired();
            builder.Property(x => x.Title).IsRequired();
        }
    }
}
