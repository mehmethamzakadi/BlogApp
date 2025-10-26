using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Seeds;

public class UserSeed : IEntityTypeConfiguration<User>
{
    private const string DefaultPasswordHash = "AQAAAAIAAYagAAAAEP8xlsKNntQQ1SivmqfdllQWKX/655QCNjrVsPYL/Oz4cUgmI8aV55GO0BN9SDNltA==";

    public void Configure(EntityTypeBuilder<User> builder)
    {
        var seedDate = new DateTime(2025, 10, 23, 7, 0, 0, DateTimeKind.Utc);
        var systemUserId = SystemUsers.SystemUserId;

        builder.HasData(
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                PasswordHash = DefaultPasswordHash,
                SecurityStamp = "b1a1d25f-8a7e-4e9a-bc55-8dca5bfa1234",
                ConcurrencyStamp = "55555555-5555-5555-5555-555555555555",
                PhoneNumber = null,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                CreatedById = systemUserId,
                CreatedDate = seedDate,
                IsDeleted = false
            },
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                UserName = "editor.lara",
                NormalizedUserName = "EDITOR.LARA",
                Email = "editor@blogapp.dev",
                NormalizedEmail = "EDITOR@BLOGAPP.DEV",
                EmailConfirmed = true,
                PasswordHash = DefaultPasswordHash,
                SecurityStamp = "0fa3f1d8-e77f-4aa9-9f12-6f8c7f90a002",
                ConcurrencyStamp = "66666666-6666-6666-6666-666666666666",
                PhoneNumber = "+905551112233",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                CreatedById = systemUserId,
                CreatedDate = seedDate,
                IsDeleted = false
            },
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                UserName = "moderator.selim",
                NormalizedUserName = "MODERATOR.SELIM",
                Email = "moderator@blogapp.dev",
                NormalizedEmail = "MODERATOR@BLOGAPP.DEV",
                EmailConfirmed = true,
                PasswordHash = DefaultPasswordHash,
                SecurityStamp = "7c1dbdbb-3d91-45a2-8578-5392cda53875",
                ConcurrencyStamp = "77777777-7777-7777-7777-777777777777",
                PhoneNumber = null,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                CreatedById = systemUserId,
                CreatedDate = seedDate,
                IsDeleted = false
            },
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                UserName = "author.melike",
                NormalizedUserName = "AUTHOR.MELIKE",
                Email = "author@blogapp.dev",
                NormalizedEmail = "AUTHOR@BLOGAPP.DEV",
                EmailConfirmed = true,
                PasswordHash = DefaultPasswordHash,
                SecurityStamp = "e8de6375-bbb3-4ac6-a5dd-8530b7072d86",
                ConcurrencyStamp = "88888888-8888-8888-8888-888888888888",
                PhoneNumber = "+905559998877",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                CreatedById = systemUserId,
                CreatedDate = seedDate,
                IsDeleted = false
            });
    }
}
