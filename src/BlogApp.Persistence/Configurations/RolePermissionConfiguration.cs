using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations;

/// <summary>
/// RolePermission entity için EF Core configuration (Many-to-Many relationship)
/// </summary>
public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        // Table name
        builder.ToTable("RolePermissions");

        // Composite primary key
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // Relationships - Optional yapıldı (global query filter uyarısını önlemek için)
        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .IsRequired(false) // ✅ Optional yapıldı
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .IsRequired(false) // ✅ Optional yapıldı
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(rp => rp.RoleId)
            .HasDatabaseName("IX_RolePermissions_RoleId");

        builder.HasIndex(rp => rp.PermissionId)
            .HasDatabaseName("IX_RolePermissions_PermissionId");
    }
}
