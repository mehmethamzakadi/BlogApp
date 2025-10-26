using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Seeds;

public class UserRoleSeed : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasData(new UserRole
        {
            UserId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            RoleId = Guid.Parse("20000000-0000-0000-0000-000000000001"),
            AssignedDate = DateTime.UtcNow
        });
    }
}
