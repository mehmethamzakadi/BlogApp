using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        // Primary key
        builder.HasKey(p => p.Id);

        // Tablo adı
        builder.ToTable("Permissions");

        // Özellikler
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.Module)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Type)
            .IsRequired()
            .HasMaxLength(50);

        // Index'ler
        builder.HasIndex(p => p.Name)
            .IsUnique()
            .HasDatabaseName("IX_Permissions_Name");

        builder.HasIndex(p => new { p.Module, p.Type })
            .HasDatabaseName("IX_Permissions_Module_Type");

        // İlişkiler
        builder.HasMany(p => p.RolePermissions)
            .WithOne(rp => rp.Permission)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
