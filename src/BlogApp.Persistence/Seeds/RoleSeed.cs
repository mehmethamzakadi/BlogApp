using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Seeds;

public class RoleSeed : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData(
            new Role
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                Name = "Admin",
                NormalizedName = "ADMIN",
                CreatedById = Guid.Empty,
                CreatedDate = DateTime.UtcNow
            },
            new Role
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                Name = "User",
                NormalizedName = "USER",
                CreatedById = Guid.Empty,
                CreatedDate = DateTime.UtcNow
            },
            new Role
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                Name = "Moderator",
                NormalizedName = "MODERATOR",
                CreatedById = Guid.Empty,
                CreatedDate = DateTime.UtcNow
            });
    }
}
