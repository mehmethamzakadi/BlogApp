using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations;

public sealed class RefreshSessionConfiguration : IEntityTypeConfiguration<RefreshSession>
{
    public void Configure(EntityTypeBuilder<RefreshSession> builder)
    {
        builder.ToTable("RefreshSessions");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasIndex(x => new { x.UserId, x.DeviceId });
        builder.HasIndex(x => x.Jti);

        builder.Property(x => x.TokenHash)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.DeviceId)
            .HasMaxLength(128);

        builder.Property(x => x.RevokedReason)
            .HasMaxLength(256);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
