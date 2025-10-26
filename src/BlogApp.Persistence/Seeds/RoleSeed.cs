using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Seeds;

public class RoleSeed : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        var seedDate = new DateTime(2025, 10, 23, 7, 0, 0, DateTimeKind.Utc);
        var systemUserId = SystemUsers.SystemUserId;

        builder.HasData(
            new Role
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                Name = UserRoles.Admin,
                NormalizedName = UserRoles.Admin.ToUpperInvariant(),
                Description = "Tüm sistemi yönetebilen ve yetki atayabilen tam yetkili rol.",
                ConcurrencyStamp = "11111111-1111-1111-1111-111111111111",
                CreatedById = systemUserId,
                CreatedDate = seedDate,
                IsDeleted = false
            },
            new Role
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                Name = UserRoles.User,
                NormalizedName = UserRoles.User.ToUpperInvariant(),
                Description = "Kendi içeriklerini oluşturup yönetebilen topluluk yazarı rolü.",
                ConcurrencyStamp = "22222222-2222-2222-2222-222222222222",
                CreatedById = systemUserId,
                CreatedDate = seedDate,
                IsDeleted = false
            },
            new Role
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                Name = "Moderator",
                NormalizedName = "MODERATOR",
                Description = "Yorumları ve topluluk etkileşimlerini yöneten rol.",
                ConcurrencyStamp = "33333333-3333-3333-3333-333333333333",
                CreatedById = systemUserId,
                CreatedDate = seedDate,
                IsDeleted = false
            },
            new Role
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000004"),
                Name = "Editor",
                NormalizedName = "EDITOR",
                Description = "Yayın akışını yöneten, içerikleri yayımlayan ve kategorileri düzenleyen rol.",
                ConcurrencyStamp = "44444444-4444-4444-4444-444444444444",
                CreatedById = systemUserId,
                CreatedDate = seedDate,
                IsDeleted = false
            });
    }
}
