using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BlogApp.Persistence.Contexts;

/// <summary>
/// Design-time factory for BlogAppDbContext - used by EF Core migrations
/// </summary>
public class BlogAppDbContextFactory : IDesignTimeDbContextFactory<BlogAppDbContext>
{
    public BlogAppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BlogAppDbContext>();

        // PostgreSQL connection string for migrations
        optionsBuilder.UseNpgsql(
            "Host=localhost;Database=BlogAppDb;Username=postgres;Password=postgres",
            b => b.MigrationsHistoryTable("__EFMigrationsHistory", "public")
        );

        // Create a mock HttpContextAccessor for design-time (null is acceptable for migrations)
        IHttpContextAccessor httpContextAccessor = null!;

        return new BlogAppDbContext(optionsBuilder.Options, httpContextAccessor);
    }
}
