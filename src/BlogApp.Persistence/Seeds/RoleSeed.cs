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
                Id = 1,
                Name = "Admin",
                NormalizedName = "ADMIN",
                CreatedDate = DateTime.UtcNow
            },
            new Role
            {
                Id = 2,
                Name = "User",
                NormalizedName = "USER",
                CreatedDate = DateTime.UtcNow
            },
            new Role
            {
                Id = 3,
                Name = "Moderator",
                NormalizedName = "MODERATOR",
                CreatedDate = DateTime.UtcNow
            });
    }
}
