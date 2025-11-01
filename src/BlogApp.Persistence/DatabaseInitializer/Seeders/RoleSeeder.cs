using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;
using Microsoft.Extensions.Logging;

namespace BlogApp.Persistence.DatabaseInitializer.Seeders;

/// <summary>
/// Role seed işlemlerini gerçekleştirir
/// Admin, User, Moderator, Editor rollerini oluşturur
/// </summary>
public class RoleSeeder : BaseSeeder
{
    public RoleSeeder(BlogAppDbContext context, ILogger<RoleSeeder> logger) 
        : base(context, logger)
    {
    }

    public override int Order => 1; // İlk önce roller oluşturulmalı
    public override string Name => "Role Seeder";

    protected override async Task SeedDataAsync(CancellationToken cancellationToken)
    {
        var createdDate = new DateTime(2025, 10, 23, 7, 0, 0, DateTimeKind.Utc);

        var roles = new List<Role>
        {
            Role.Create(UserRoles.Admin, "Sistem yöneticisi - tüm yetkilere sahip"),
            Role.Create(UserRoles.User, "Normal kullanıcı - temel yetkilere sahip"),
            Role.Create(UserRoles.Moderator, "İçerik moderatörü - içerik yönetimi yetkileri"),
            Role.Create(UserRoles.Editor, "İçerik editörü - post ve kategori yönetimi yetkileri")
        };

        // Sabit ID'leri ve tarihleri EF Core ile set et
        var roleIds = new[]
        {
            Guid.Parse("20000000-0000-0000-0000-000000000001"),
            Guid.Parse("20000000-0000-0000-0000-000000000002"),
            Guid.Parse("20000000-0000-0000-0000-000000000003"),
            Guid.Parse("20000000-0000-0000-0000-000000000004")
        };

        for (int i = 0; i < roles.Count; i++)
        {
            var entry = Context.Entry(roles[i]);
            entry.Property("Id").CurrentValue = roleIds[i];
            entry.Property("CreatedDate").CurrentValue = createdDate;
            entry.Property("IsDeleted").CurrentValue = false;
        }

        await AddRangeIfNotExistsAsync(roles, r => (Guid)Context.Entry(r).Property("Id").CurrentValue!, cancellationToken);
    }
}
