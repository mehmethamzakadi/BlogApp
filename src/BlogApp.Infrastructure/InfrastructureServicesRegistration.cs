using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Options;
using BlogApp.Infrastructure.Authorization;
using BlogApp.Infrastructure.Consumers;
using BlogApp.Infrastructure.Services;
using BlogApp.Infrastructure.Services.Identity;
using BlogApp.Persistence.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TokenOptions = BlogApp.Domain.Options.TokenOptions;

namespace BlogApp.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TokenOptions>(configuration.GetSection(TokenOptions.SectionName));
            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
            services.Configure<PasswordResetOptions>(configuration.GetSection(PasswordResetOptions.SectionName));
            services.Configure<TelegramOptions>(configuration.GetSection(TelegramOptions.SectionName));
            services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;

                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@";
                options.User.RequireUniqueEmail = true;
            })
                .AddErrorDescriber<TurkishIdentityErrorDescriber>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddEntityFrameworkStores<BlogAppDbContext>()
                .AddDefaultTokenProviders();

            TokenOptions tokenOptions = configuration.GetSection(TokenOptions.SectionName).Get<TokenOptions>()
                ?? throw new InvalidOperationException("Token ayarları yapılandırılmalıdır.");

            var environment = configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT");
            bool requireHttpsMetadata = !string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = requireHttpsMetadata;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = tokenOptions.Audience,
                    ValidIssuer = tokenOptions.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            var redisConnectionString = configuration.GetConnectionString("RedisCache");
            if (!string.IsNullOrWhiteSpace(redisConnectionString))
            {
                services.AddStackExchangeRedisCache(options =>
                    options.Configuration = redisConnectionString);
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            services.AddMassTransit(x =>
            {
                x.AddConsumer<SendTelgeramMessageConsumer>();
                x.AddConsumer<ActivityLogConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                    cfg.Host(rabbitOptions.HostName, "/", hostConfigurator =>
                    {
                        hostConfigurator.Username(rabbitOptions.UserName);
                        hostConfigurator.Password(rabbitOptions.Password);
                    });

                    // Telegram message queue
                    cfg.ReceiveEndpoint(EventConstants.SendTelegramTextMessageQueue, endpointConfigurator =>
                    {
                        endpointConfigurator.ConfigureConsumer<SendTelgeramMessageConsumer>(context);
                    });

                    // Activity Log queue with retry and error handling
                    cfg.ReceiveEndpoint(EventConstants.ActivityLogQueue, endpointConfigurator =>
                    {
                        endpointConfigurator.ConfigureConsumer<ActivityLogConsumer>(context);

                        // Retry configuration
                        endpointConfigurator.UseMessageRetry(retryConfigurator =>
                            retryConfigurator.Exponential(5,
                                TimeSpan.FromSeconds(1),
                                TimeSpan.FromMinutes(5),
                                TimeSpan.FromSeconds(2)));

                        // Concurrency settings
                        endpointConfigurator.PrefetchCount = 16;
                        endpointConfigurator.ConcurrentMessageLimit = 8;
                    });

                    if (rabbitOptions.RetryLimit > 0)
                    {
                        cfg.UseMessageRetry(retryConfigurator => retryConfigurator.Immediate(rabbitOptions.RetryLimit));
                    }
                });
            });

            services.AddScoped<SendTelgeramMessageConsumer>();
            services.AddScoped<ActivityLogConsumer>();

            // Background Services
            services.AddHostedService<Services.BackgroundServices.OutboxProcessorService>();

            services.AddSingleton<ITelegramService, TelegramService>();
            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddTransient<ITokenService, JwtTokenService>();
            services.AddTransient<IMailService, MailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Authorization
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddAuthorizationCore(options =>
            {
                // Permission'lar için policy'ler oluştur
                foreach (var permission in Permissions.GetAllPermissions())
                {
                    options.AddPolicy(permission, policy =>
                        policy.Requirements.Add(new PermissionRequirement(permission)));
                }
            });

            // Register log cleanup background service
            services.AddHostedService<LogCleanupService>();

            return services;
        }
    }
}
