using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Persistence.Contexts;
using BlogApp.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Persistence
{
    public static class PersistenceServicesRegistration
    {
        public static IServiceCollection AddConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BlogAppDbContext>(options =>
               options.UseNpgsql(
                   configuration.GetConnectionString("BlogAppConnectionString")));


            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();

            return services;
        }
    }
}
