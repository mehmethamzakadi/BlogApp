using BlogApp.Application.Interfaces.Infrastructure;
using BlogApp.Infrastructure.TelegramBot;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddConfigureInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<ITelegramBotManager, TelegramBotManager>();

            return services;
        }
    }
}
