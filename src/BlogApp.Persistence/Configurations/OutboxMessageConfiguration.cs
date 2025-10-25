using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventType)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Payload)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.ProcessedAt);

        builder.Property(x => x.RetryCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.Error)
            .HasMaxLength(2000);

        builder.Property(x => x.NextRetryAt);

        // Performans için index'ler
        builder.HasIndex(x => x.ProcessedAt)
            .HasDatabaseName("IX_OutboxMessages_ProcessedAt");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_OutboxMessages_CreatedAt");

        builder.HasIndex(x => new { x.ProcessedAt, x.NextRetryAt })
            .HasDatabaseName("IX_OutboxMessages_ProcessedAt_NextRetryAt");
    }
}
