using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Seeds;

public class UserRoleSeed : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        var assignedDate = new DateTime(2025, 10, 23, 7, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new UserRole
            {
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                RoleId = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                AssignedDate = assignedDate
            },
            new UserRole
            {
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                RoleId = Guid.Parse("20000000-0000-0000-0000-000000000004"),
                AssignedDate = assignedDate
            },
            new UserRole
            {
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                RoleId = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                AssignedDate = assignedDate
            },
            new UserRole
            {
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                RoleId = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                AssignedDate = assignedDate
            },
            new UserRole
            {
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                RoleId = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                AssignedDate = assignedDate
            },
            new UserRole
            {
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                RoleId = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                AssignedDate = assignedDate
            });
    }
}
