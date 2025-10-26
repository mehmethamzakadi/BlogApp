using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Seeds;

public class UserSeed : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        var user = new User
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Email = "admin@admin.com",
            NormalizedEmail = "ADMIN@ADMIN.COM",
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            PhoneNumberConfirmed = false,
            LockoutEnabled = false,
            EmailConfirmed = false,
            TwoFactorEnabled = false,
            SecurityStamp = Guid.Parse("b1a1d25f-8a7e-4e9a-bc55-8dca5bfa1234").ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            AccessFailedCount = 0,
            CreatedById = Guid.Empty,
            CreatedDate = DateTime.UtcNow,
            PasswordHash = string.Empty // Will be set below
        };

        // Hash password using Identity's PasswordHasher for consistency
        var password = new PasswordHasher<User>();
        var hashed = password.HashPassword(user, "mAdmin92");
        user.PasswordHash = hashed;

        builder.HasData(user);
    }
}
