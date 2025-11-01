using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;
using Microsoft.Extensions.Logging;

namespace BlogApp.Persistence.DatabaseInitializer.Seeders;

/// <summary>
/// Örnek kategori verilerini seed eder
/// </summary>
public class CategorySeeder : BaseSeeder
{
    public CategorySeeder(BlogAppDbContext context, ILogger<CategorySeeder> logger) 
        : base(context, logger)
    {
    }

    public override int Order => 6; // UserRole'den sonra
    public override string Name => "Category Seeder";

    protected override async Task SeedDataAsync(CancellationToken cancellationToken)
    {
        var seedDate = new DateTime(2025, 10, 23, 7, 0, 0, DateTimeKind.Utc);
        var systemUserId = SystemUsers.SystemUserId;

        var categoryDataList = new[]
        {
            new { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Technology" },
            new { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Programming" },
            new { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "Design" },
            new { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Name = "Lifestyle" },
            new { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Name = "Travel" }
        };

        var categories = new List<Category>();

        foreach (var categoryData in categoryDataList)
        {
            var category = Category.Create(categoryData.Name);

            // Sabit ID ve tarihleri EF Core ile set et
            var entry = Context.Entry(category);
            entry.Property("Id").CurrentValue = categoryData.Id;
            entry.Property("CreatedDate").CurrentValue = seedDate;
            entry.Property("CreatedById").CurrentValue = systemUserId;
            entry.Property("IsDeleted").CurrentValue = false;

            categories.Add(category);
        }

        await AddRangeIfNotExistsAsync(categories, c => (Guid)Context.Entry(c).Property("Id").CurrentValue!, cancellationToken);
    }
}
