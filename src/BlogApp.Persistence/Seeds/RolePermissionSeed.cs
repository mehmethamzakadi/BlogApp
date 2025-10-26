using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Seeds;

public class RolePermissionSeed : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        var grantedAt = new DateTime(2025, 10, 23, 7, 0, 0, DateTimeKind.Utc);

        var permissionIdMap = Permissions.GetAllPermissions()
            .Select((permissionName, index) => new
            {
                Name = permissionName,
                Id = Guid.Parse($"30000000-0000-0000-0000-000000000{index + 1:D3}")
            })
            .ToDictionary(x => x.Name, x => x.Id);

        var adminRoleId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var userRoleId = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var moderatorRoleId = Guid.Parse("20000000-0000-0000-0000-000000000003");
        var editorRoleId = Guid.Parse("20000000-0000-0000-0000-000000000004");

        var rolePermissions = new List<RolePermission>();

        void AddPermissions(Guid roleId, IEnumerable<string> permissionNames)
        {
            foreach (var name in permissionNames.Distinct())
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionIdMap[name],
                    GrantedAt = grantedAt
                });
            }
        }

        AddPermissions(adminRoleId, Permissions.GetAllPermissions());

        AddPermissions(editorRoleId, new[]
        {
            Permissions.PostsCreate,
            Permissions.PostsRead,
            Permissions.PostsUpdate,
            Permissions.PostsDelete,
            Permissions.PostsViewAll,
            Permissions.PostsPublish,
            Permissions.CategoriesCreate,
            Permissions.CategoriesRead,
            Permissions.CategoriesUpdate,
            Permissions.CategoriesViewAll,
            Permissions.CommentsRead,
            Permissions.CommentsModerate,
            Permissions.CommentsDelete,
            Permissions.DashboardView
        });

        AddPermissions(moderatorRoleId, new[]
        {
            Permissions.CommentsRead,
            Permissions.CommentsViewAll,
            Permissions.CommentsModerate,
            Permissions.CommentsDelete,
            Permissions.PostsRead
        });

        AddPermissions(userRoleId, new[]
        {
            Permissions.PostsCreate,
            Permissions.PostsRead,
            Permissions.PostsUpdate,
            Permissions.CategoriesRead,
            Permissions.CategoriesViewAll,
            Permissions.CommentsCreate,
            Permissions.CommentsRead,
            Permissions.CommentsUpdate
        });

        builder.HasData(rolePermissions);
    }
}
