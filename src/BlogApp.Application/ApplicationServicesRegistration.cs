using BlogApp.Application.Behaviors;
using BlogApp.Application.Options;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace BlogApp.Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TokenOptions>(configuration.GetSection("TokenOptions"));

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                
                // Pipeline behaviors - sıralama önemli!
                // 1. Validation - en başta
                configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
                // 2. Logging
                configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
                // 3. Cache invalidation
                configuration.AddOpenBehavior(typeof(CacheInvalidationBehavior<,>));
                // 4. Concurrency - en sonda (retry mekanizması tüm pipeline'ı tekrar çalıştırır)
                configuration.AddOpenBehavior(typeof(ConcurrencyBehavior<,>));
            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
