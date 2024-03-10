using BlogApp.Domain.Constants;
using BlogApp.Infrastructure.RabbitMq.Consumers;
using BlogApp.Infrastructure.TelegramBot;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITelegramBotManager, TelegramBotManager>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<SendTextMessageConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var host = configuration.GetSection("RabbitMQ")["HostName"];
                    var userName = configuration.GetSection("RabbitMQ")["Username"];
                    var password = configuration.GetSection("RabbitMQ")["Password"];
                    var retryLimit = Convert.ToInt32(configuration.GetSection("RabbitMQ")["RetryLimit"]);

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

            return services;
        }
    }
}
