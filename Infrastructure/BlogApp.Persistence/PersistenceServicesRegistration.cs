using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;
using BlogApp.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Persistence
{
    public static class PersistenceServicesRegistration
    {
        public static IServiceCollection AddConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BlogAppDbContext>(options =>
               options.UseNpgsql(
                   configuration.GetConnectionString("BlogAppPostgreConnectionString")));
            services.AddIdentityCore<AppUser>().AddEntityFrameworkStores<BlogAppDbContext>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();

            return services;
        }
    }
}
