using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations;

public class BookshelfItemConfiguration : BaseConfiguraiton<BookshelfItem>
{
    public override void Configure(EntityTypeBuilder<BookshelfItem> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Author)
            .HasMaxLength(150);

        builder.Property(x => x.Publisher)
            .HasMaxLength(150);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.PageCount);

        builder.Property(x => x.IsRead)
            .IsRequired();

        builder.Property(x => x.ReadDate);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(400);

        builder.HasIndex(x => x.IsRead)
            .HasDatabaseName("IX_BookshelfItems_IsRead");

        builder.HasIndex(x => x.ReadDate)
            .HasDatabaseName("IX_BookshelfItems_ReadDate");

        builder.HasIndex(x => x.CreatedDate)
            .HasDatabaseName("IX_BookshelfItems_CreatedDate");

        builder.HasIndex(x => x.Title)
            .HasDatabaseName("IX_BookshelfItems_Title");
    }
}
