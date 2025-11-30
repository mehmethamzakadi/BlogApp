
using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations
{
    public class CategoryConfiguration : BaseConfiguraiton<Category>
    {
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

            builder.Property(x => x.NormalizedName)
                .HasMaxLength(100);  // Geçici olarak nullable - migration sonrası required yapılacak

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            // Parent-Child relationship (self-referencing)
            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict); // Parent silinirse child'lar silinmesin

            // Indexler - NormalizedName üzerinden unique index (case-insensitive)
            // Sadece silinmemiş kayıtlar için unique olsun
            builder.HasIndex(x => x.NormalizedName)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false")
                .HasDatabaseName("IX_Categories_NormalizedName_Unique");

            // Name için normal index (arama için)
            builder.HasIndex(x => x.Name)
                .HasDatabaseName("IX_Categories_Name");

            // ParentId için index (hierarchical sorgular için)
            builder.HasIndex(x => x.ParentId)
                .HasDatabaseName("IX_Categories_ParentId");
        }
    }
}
