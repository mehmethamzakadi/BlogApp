using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;
using Microsoft.Extensions.Logging;

namespace BlogApp.Persistence.DatabaseInitializer.Seeders;

/// <summary>
/// Test ve demo kullanıcılarını seed eder
/// </summary>
public class UserSeeder : BaseSeeder
{
    // Default şifre: Admin123! (hashed)
    private const string DefaultPasswordHash = "AQAAAAIAAYagAAAAEP8xlsKNntQQ1SivmqfdllQWKX/655QCNjrVsPYL/Oz4cUgmI8aV55GO0BN9SDNltA==";

    public UserSeeder(BlogAppDbContext context, ILogger<UserSeeder> logger) 
        : base(context, logger)
    {
    }

    public override int Order => 4; // Rollerden sonra
    public override string Name => "User Seeder";

    protected override async Task SeedDataAsync(CancellationToken cancellationToken)
    {
        var seedDate = new DateTime(2025, 10, 23, 7, 0, 0, DateTimeKind.Utc);
        var systemUserId = SystemUsers.SystemUserId;

        var userDataList = new[]
        {
            new { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), UserName = "admin", Email = "admin@admin.com", Phone = (string?)null, PhoneConfirmed = false, EmailConfirmed = true },
            new { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), UserName = "editor_lara", Email = "editor@blogapp.dev", Phone = (string?)"+905551112233", PhoneConfirmed = true, EmailConfirmed = true },
            new { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), UserName = "moderator_selim", Email = "moderator@blogapp.dev", Phone = (string?)null, PhoneConfirmed = false, EmailConfirmed = true },
            new { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), UserName = "author_melike", Email = "author@blogapp.dev", Phone = (string?)"+905559998877", PhoneConfirmed = true, EmailConfirmed = true }
        };

        var users = new List<User>();

        foreach (var userData in userDataList)
        {
            var user = User.Create(
                userName: userData.UserName,
                email: userData.Email,
                passwordHash: DefaultPasswordHash
            );

            // Sabit ID, tarihleri ve diğer özellikleri EF Core ile set et
            var entry = Context.Entry(user);
            entry.Property("Id").CurrentValue = userData.Id;
            entry.Property("CreatedDate").CurrentValue = seedDate;
            entry.Property("CreatedById").CurrentValue = systemUserId;
            entry.Property("IsDeleted").CurrentValue = false;
            entry.Property(nameof(User.EmailConfirmed)).CurrentValue = userData.EmailConfirmed;
            entry.Property(nameof(User.PhoneNumber)).CurrentValue = userData.Phone;
            entry.Property(nameof(User.PhoneNumberConfirmed)).CurrentValue = userData.PhoneConfirmed;

            users.Add(user);
        }

        await AddRangeIfNotExistsAsync(users, u => (Guid)Context.Entry(u).Property("Id").CurrentValue!, cancellationToken);
    }
}
