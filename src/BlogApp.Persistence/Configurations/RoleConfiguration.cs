using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations;

/// <summary>
/// Role entity için EF Core configuration
/// </summary>
public class RoleConfiguration : BaseConfiguraiton<Role>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);

        // Table name
        builder.ToTable("Roles");

        // Name
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(r => r.NormalizedName)
            .IsRequired()
            .HasMaxLength(150);

        // Concurrency
        builder.Property(r => r.ConcurrencyStamp)
            .IsConcurrencyToken()
            .IsRequired()
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(r => r.NormalizedName)
            .IsUnique()
            .HasDatabaseName("IX_Roles_NormalizedName");

        builder.HasIndex(r => r.Name)
            .HasDatabaseName("IX_Roles_Name");
    }
}
