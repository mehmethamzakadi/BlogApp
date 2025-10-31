
using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations
{
    public class PostConfiguration : BaseConfiguraiton<Post>
    {
        public override void Configure(EntityTypeBuilder<Post> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Body).IsRequired();
            builder.Property(x => x.Summary).IsRequired();
            builder.Property(x => x.Thumbnail).IsRequired();
            builder.Property(x => x.Title).IsRequired();

            // Relationships
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.IsPublished, x.CategoryId, x.CreatedDate })
                .HasDatabaseName("IX_Posts_IsPublished_CategoryId_CreatedDate");

            builder.HasIndex(x => x.IsDeleted)
                .HasFilter("\"IsDeleted\" = false")
                .HasDatabaseName("IX_Posts_NotDeleted");

            builder.HasIndex(x => x.Title)
                .HasDatabaseName("IX_Posts_Title");
        }
    }
}
