using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using BlogApp.Persistence.DatabaseInitializer;
using BlogApp.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Persistence;

public static class PersistenceServicesRegistration
{
    public static IServiceCollection AddConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        #region DbContext Configuration
        var postgreSqlConnectionString = configuration.GetConnectionString("BlogAppPostgreConnectionString");
        services.AddDbContext<BlogAppDbContext>(options => options.UseNpgsql(postgreSqlConnectionString));
        #endregion

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IDbInitializer, DbInitializer>();

        return services;
    }
}