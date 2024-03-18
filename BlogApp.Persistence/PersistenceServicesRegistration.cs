using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using BlogApp.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlogApp.Persistence;

public static class PersistenceServicesRegistration
{
    public static IServiceCollection AddConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        #region DbContext Configuration
        var mssqlConnectionString = configuration.GetConnectionString("BlogAppMsSqlConnectionString");
        var postgreSqlConnectionString = configuration.GetConnectionString("BlogAppPostgreConnectionString");

        services.AddDbContext<BlogAppDbContext>(options =>
         options.UseSqlServer(mssqlConnectionString)
         /*options.UseNpgsql(postgreSqlConnectionString)*/
         );
        #endregion

        #region Identity Configurtaion
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
            .AddRoleManager<RoleManager<AppRole>>()
            .AddEntityFrameworkStores<BlogAppDbContext>()
            .AddDefaultTokenProviders();
        #endregion

        #region Authentication With Jwt
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidAudience = configuration["TokenOptions:Audience"],
                ValidIssuer = configuration["TokenOptions:Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenOptions:SecurityKey"] ?? string.Empty)),
                ClockSkew = TimeSpan.Zero
            };
        });
        #endregion

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IPostRepository, PostRepository>();

        return services;
    }
}