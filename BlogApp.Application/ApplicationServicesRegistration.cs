using BlogApp.Application.Behaviors.Logging;
using BlogApp.Application.Behaviors.Transaction;
using BlogApp.Domain.Options;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace BlogApp.Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TelegramOptions>(configuration.GetSection("TelegramBotOptions"));
            services.Configure<TokenOptions>(configuration.GetSection("TokenOptions"));

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                configuration.AddOpenBehavior(typeof(TransactionScopeBehavior<,>));
                configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });

            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
