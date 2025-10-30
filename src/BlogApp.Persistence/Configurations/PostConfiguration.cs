
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

            // Indexler
            builder.HasIndex(x => x.CategoryId)
                .HasDatabaseName("IX_Posts_CategoryId");

            builder.HasIndex(x => x.IsPublished)
                .HasDatabaseName("IX_Posts_IsPublished");

            builder.HasIndex(x => x.CreatedDate)
                .HasDatabaseName("IX_Posts_CreatedDate");

            builder.HasIndex(x => new { x.IsPublished, x.CreatedDate })
                .HasDatabaseName("IX_Posts_IsPublished_CreatedDate");

            builder.HasIndex(x => x.Title)
                .HasDatabaseName("IX_Posts_Title");
        }
    }
}
