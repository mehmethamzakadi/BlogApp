using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations;

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("ActivityLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ActivityType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.EntityType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Details)
            .HasMaxLength(2000);

        builder.Property(a => a.Timestamp)
            .IsRequired();

        // User ilişkisi optional yapıldı (global query filter uyarısını önlemek için)
        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .IsRequired(false) // ✅ Optional yapıldı
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(a => new { a.UserId, a.Timestamp })
            .HasDatabaseName("IX_ActivityLogs_UserId_Timestamp");

        builder.HasIndex(a => a.Timestamp)
            .HasDatabaseName("IX_ActivityLogs_Timestamp");

        builder.HasIndex(a => new { a.EntityType, a.EntityId })
            .HasDatabaseName("IX_ActivityLogs_Entity");
    }
}
