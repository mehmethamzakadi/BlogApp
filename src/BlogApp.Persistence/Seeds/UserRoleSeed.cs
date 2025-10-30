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
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                RoleId = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                AssignedDate = assignedDate,
                CreatedDate = assignedDate,
                UpdatedDate = assignedDate
            },
            new UserRole
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                RoleId = Guid.Parse("20000000-0000-0000-0000-000000000004"),
                AssignedDate = assignedDate,
                CreatedDate = assignedDate,
                UpdatedDate = assignedDate
            },
            new UserRole
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                RoleId = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                AssignedDate = assignedDate,
                CreatedDate = assignedDate,
                UpdatedDate = assignedDate
            },
            new UserRole
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000004"),
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                RoleId = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                AssignedDate = assignedDate,
                CreatedDate = assignedDate,
                UpdatedDate = assignedDate
            },
            new UserRole
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000005"),
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                RoleId = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                AssignedDate = assignedDate,
                CreatedDate = assignedDate,
                UpdatedDate = assignedDate
            },
            new UserRole
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000006"),
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                RoleId = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                AssignedDate = assignedDate,
                CreatedDate = assignedDate,
                UpdatedDate = assignedDate
            });
    }
}
