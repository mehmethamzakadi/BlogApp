using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Application.Abstractions.Images;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Options;
using BlogApp.Infrastructure.Authorization;
using BlogApp.Infrastructure.Consumers;
using BlogApp.Infrastructure.Services;
using BlogApp.Infrastructure.Services.BackgroundServices.Outbox.Converters;
using BlogApp.Infrastructure.Services.Images;
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
            services.Configure<ImageStorageOptions>(configuration.GetSection(ImageStorageOptions.SectionName));

            // Custom Password Hasher for User entity
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

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
                    ClockSkew = TimeSpan.FromSeconds(30)
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

            services.AddSingleton<IIntegrationEventConverterStrategy, CategoryCreatedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, CategoryUpdatedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, CategoryDeletedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, BookshelfItemCreatedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, BookshelfItemUpdatedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, BookshelfItemDeletedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, PostCreatedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, PostUpdatedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, PostDeletedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, UserCreatedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, UserUpdatedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, UserDeletedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, UserRolesAssignedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, RoleCreatedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, RoleUpdatedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, RoleDeletedIntegrationEventConverter>();
            services.AddSingleton<IIntegrationEventConverterStrategy, PermissionsAssignedToRoleIntegrationEventConverter>();

            services.AddSingleton<ITelegramService, TelegramService>();
            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddTransient<ITokenService, JwtTokenService>();
            services.AddTransient<IMailService, MailService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IImageStorageService, ImageStorageService>();

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
