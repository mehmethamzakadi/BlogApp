
using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
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

            // Indexler
            builder.HasIndex(x => x.PostId)
                .HasDatabaseName("IX_Comments_PostId");

            builder.HasIndex(x => x.ParentId)
                .HasDatabaseName("IX_Comments_ParentId");

            builder.HasIndex(x => x.IsPublished)
                .HasDatabaseName("IX_Comments_IsPublished");

            builder.HasIndex(x => new { x.PostId, x.IsPublished })
                .HasDatabaseName("IX_Comments_PostId_IsPublished");

            builder.HasIndex(x => x.CreatedDate)
                .HasDatabaseName("IX_Comments_CreatedDate");
        }
    }
}
