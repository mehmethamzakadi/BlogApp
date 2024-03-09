using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using BlogApp.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Persistence;

public static class PersistenceServicesRegistration
{
    public static IServiceCollection AddConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var mssqlConnectionString = configuration.GetConnectionString("BlogAppMsSqlConnectionString");
        var postgreSqlConnectionString = configuration.GetConnectionString("BlogAppPostgreConnectionString");

        services.AddDbContext<BlogAppDbContext>(options =>
         options.UseSqlServer(mssqlConnectionString)
         /*options.UseNpgsql(postgreSqlConnectionString)*/
         );

        services.AddIdentity<AppUser, AppRole>(options =>
        {
            //User Şifre Ayarları
            options.Password.RequireDigit = true; //Sayı girme zorunluluğu
            options.Password.RequiredLength = 6; //Minimum şifre uzunluğu.
            options.Password.RequiredUniqueChars = 0; //Özel karakter bulundurma sayısı.
            options.Password.RequireNonAlphanumeric = false; //Özel karakter bulundurma zorunluluğu.
            options.Password.RequireLowercase = false; //Küçük harf bulundurma zorunluluğu.
            options.Password.RequireUppercase = false; //Büyük harf bulundurma zorunluluğu.

            //User Ayarları
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@"; //Bu karakterler dışında kullanım yapılamaz.
            options.User.RequireUniqueEmail = true; //Tek mail adresi ile kayıt olabilme.
        })
            .AddEntityFrameworkStores<BlogAppDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IPostRepository, PostRepository>();

        return services;
    }
}
