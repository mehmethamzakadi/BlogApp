﻿using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;
using BlogApp.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlogApp.Persistence
{
    public static class PersistenceServicesRegistration
    {
        public static IServiceCollection AddConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BlogAppDbContext>(options =>
               options.UseNpgsql(
                   configuration.GetConnectionString("BlogAppPostgreConnectionString")));

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
                .AddEntityFrameworkStores<BlogAppDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IAppUserTokenRepository, AppUserTokenRepository>();


            return services;
        }
    }
}