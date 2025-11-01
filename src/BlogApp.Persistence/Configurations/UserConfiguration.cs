using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations;

/// <summary>
/// User entity için EF Core configuration
/// </summary>
public class UserConfiguration : BaseConfiguraiton<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        // Table name
        builder.ToTable("Users");

        // UserName - backing field only
        builder.Property("_userName")
            .HasColumnName("UserName")
            .IsRequired()
            .HasMaxLength(100);

        builder.Ignore(u => u.UserName);

        builder.Property(u => u.NormalizedUserName)
            .IsRequired()
            .HasMaxLength(100)
            .HasComputedColumnSql("UPPER(\"UserName\")", stored: true)
            .ValueGeneratedOnAddOrUpdate();

        // Email - backing field only
        builder.Property("_email")
            .HasColumnName("Email")
            .IsRequired()
            .HasMaxLength(200);

        builder.Ignore(u => u.Email);

        builder.Property(u => u.NormalizedEmail)
            .IsRequired()
            .HasMaxLength(200)
            .HasComputedColumnSql("UPPER(\"Email\")", stored: true)
            .ValueGeneratedOnAddOrUpdate();

        // Password
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        // Security & Concurrency
        builder.Property(u => u.SecurityStamp)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.ConcurrencyStamp)
            .IsConcurrencyToken()
            .IsRequired()
            .HasMaxLength(100);

        // Phone
        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(50);

        // Password Reset Token
        builder.Property(u => u.PasswordResetToken)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(u => u.NormalizedUserName)
            .IsUnique()
            .HasDatabaseName("IX_Users_NormalizedUserName");

        builder.HasIndex(u => u.NormalizedEmail)
            .IsUnique()
            .HasDatabaseName("IX_Users_NormalizedEmail");

        // Password reset token index - sadece aktif token'lar için (nullable olmayan)
        builder.HasIndex(u => u.PasswordResetToken)
            .HasFilter("\"PasswordResetToken\" IS NOT NULL")
            .HasDatabaseName("IX_Users_PasswordResetToken");

    }
}
