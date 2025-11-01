using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlogApp.Persistence.DatabaseInitializer.Seeders;

/// <summary>
/// UserRole ilişkilerini seed eder
/// Kullanıcılara roller atar
/// </summary>
public class UserRoleSeeder : BaseSeeder
{
    public UserRoleSeeder(BlogAppDbContext context, ILogger<UserRoleSeeder> logger) 
        : base(context, logger)
    {
    }

    public override int Order => 5; // User ve Role'den sonra
    public override string Name => "UserRole Seeder";

    protected override async Task SeedDataAsync(CancellationToken cancellationToken)
    {
        var assignedAt = new DateTime(2025, 10, 23, 7, 0, 0, DateTimeKind.Utc);

        // User ve Role ID'leri
        var adminUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var editorUserId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var moderatorUserId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        var authorUserId = Guid.Parse("00000000-0000-0000-0000-000000000004");

        var adminRoleId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var userRoleId = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var moderatorRoleId = Guid.Parse("20000000-0000-0000-0000-000000000003");
        var editorRoleId = Guid.Parse("20000000-0000-0000-0000-000000000004");

        var userRoles = new List<UserRole>
        {
            new UserRole { UserId = adminUserId, RoleId = adminRoleId, AssignedDate = assignedAt, IsDeleted = false },
            new UserRole { UserId = editorUserId, RoleId = editorRoleId, AssignedDate = assignedAt, IsDeleted = false },
            new UserRole { UserId = moderatorUserId, RoleId = moderatorRoleId, AssignedDate = assignedAt, IsDeleted = false },
            new UserRole { UserId = authorUserId, RoleId = userRoleId, AssignedDate = assignedAt, IsDeleted = false }
        };

        // Mevcut user-role ilişkilerini kontrol et
        var existingRelations = await Context.UserRoles
            .Select(ur => new { ur.UserId, ur.RoleId })
            .ToHashSetAsync(cancellationToken);

        var newUserRoles = userRoles
            .Where(ur => !existingRelations.Contains(new { ur.UserId, ur.RoleId }))
            .ToList();

        if (newUserRoles.Any())
        {
            await Context.UserRoles.AddRangeAsync(newUserRoles, cancellationToken);
            Logger.LogInformation("Added {Count} new UserRole relations", newUserRoles.Count);
        }
        else
        {
            Logger.LogInformation("All UserRole relations already exist, skipping");
        }
    }
}
