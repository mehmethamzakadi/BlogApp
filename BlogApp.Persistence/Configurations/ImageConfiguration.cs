using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations
{
    public class ImageConfiguration : BaseConfiguraiton<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Path).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Type).IsRequired().HasMaxLength(20);
        }
    }
}
