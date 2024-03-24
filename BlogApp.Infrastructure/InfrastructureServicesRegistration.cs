using BlogApp.Application.Abstractions;
using BlogApp.Domain.AppSettingsOptions;
using BlogApp.Domain.Constants;
using BlogApp.Infrastructure.RabbitMq.Consumers;
using BlogApp.Infrastructure.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Yapılandırma ayarlarını okumak için
            services.Configure<TelegramOptions>(configuration.GetSection("TelegramBotOptions"));

            services.AddSingleton<ITelegramService, TelegramService>();
            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddTransient<ITokenService, JwtTokenService>();

            #region Redis Configurations
            services.AddStackExchangeRedisCache(options =>
              options.Configuration = configuration.GetConnectionString("RedisCache"));
            #endregion

            #region MassTransit RabbitMq Configurations
            services.AddMassTransit(x =>
            {
                x.AddConsumer<SendTextMessageConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var host = configuration.GetSection("RabbitMQOptions")["HostName"];
                    var userName = configuration.GetSection("RabbitMQOptions")["Username"];
                    var password = configuration.GetSection("RabbitMQOptions")["Password"];
                    var retryLimit = Convert.ToInt32(configuration.GetSection("RabbitMQOptions")["RetryLimit"]);

                    cfg.Host(host, "/", conf =>
                    {
                        conf.Username(userName);
                        conf.Password(password);

                        cfg.ReceiveEndpoint(EventConstants.SendTelegramTextMessageQueue,
                            c =>
                            {
                                c.ConfigureConsumer<SendTextMessageConsumer>(context);
                            });
                        cfg.UseMessageRetry(r => r.Immediate(retryLimit));
                    });
                });
            });

            services.AddScoped<SendTextMessageConsumer>();
            #endregion

            return services;
        }
    }
}
