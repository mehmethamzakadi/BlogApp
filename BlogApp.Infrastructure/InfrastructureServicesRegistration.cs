using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Infrastructure.Consumers;
using BlogApp.Infrastructure.Services;
using BlogApp.Infrastructure.Services.Identity;
using BlogApp.Persistence.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlogApp.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
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

            #region Redis Configurations
            services.AddStackExchangeRedisCache(options =>
              options.Configuration = configuration.GetConnectionString("RedisCache"));
            #endregion

            #region MassTransit RabbitMq Configurations
            services.AddMassTransit(x =>
            {
                x.AddConsumer<SendTelgeramMessageConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var host = configuration.GetSection("RabbitMQOptions")["HostName"];
                    var userName = configuration.GetSection("RabbitMQOptions")["Username"]!;
                    var password = configuration.GetSection("RabbitMQOptions")["Password"]!;
                    var retryLimit = Convert.ToInt32(configuration.GetSection("RabbitMQOptions")["RetryLimit"]);

                    cfg.Host(host, "/", conf =>
                    {
                        conf.Username(userName);
                        conf.Password(password);

                        cfg.ReceiveEndpoint(EventConstants.SendTelegramTextMessageQueue,
                            c =>
                            {
                                c.ConfigureConsumer<SendTelgeramMessageConsumer>(context);
                            });
                        cfg.UseMessageRetry(r => r.Immediate(retryLimit));
                    });
                });
            });

            services.AddScoped<SendTelgeramMessageConsumer>();
            #endregion

            services.AddSingleton<ITelegramService, TelegramService>();
            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddTransient<ITokenService, JwtTokenService>();
            services.AddTransient<IMailService, MailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();

            return services;
        }
    }
}
