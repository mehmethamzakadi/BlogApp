using BlogApp.Domain.Common;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using BlogApp.Persistence.DatabaseInitializer;
using BlogApp.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Persistence;

public static class PersistenceServicesRegistration
{
    public static IServiceCollection AddConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        #region DbContext Configuration
        var postgreSqlConnectionString = configuration.GetConnectionString("BlogAppPostgreConnectionString");

        services.AddDbContextPool<BlogAppDbContext>((sp, options) =>
        {
            options.UseNpgsql(postgreSqlConnectionString);
            options.EnableServiceProviderCaching();
            options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)); // ✅
        });

        #endregion

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IDbInitializer, DbInitializer>();

        // Unit of Work registration
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}