
using System;
using BlogApp.Application;
using BlogApp.Infrastructure;
using BlogApp.Persistence;
using BlogApp.Persistence.DatabaseInitializer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var corsPolicyName = "_dynamicCorsPolicy";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddConfigurePersistenceServices(builder.Configuration);
builder.Services.AddConfigureApplicationServices(builder.Configuration);
builder.Services.AddConfigureInfrastructureServices(builder.Configuration);

builder.Services.AddOptions();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName, policyBuilder =>
    {
        if (allowedOrigins.Length == 0)
        {
            policyBuilder.AllowAnyOrigin();
        }
        else
        {
            policyBuilder.WithOrigins(allowedOrigins);
        }

        policyBuilder.AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Blog App API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                }
            },
            Array.Empty<string>()
        }
    });
});

var isDevelopment = builder.Environment.IsDevelopment();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie = new CookieBuilder
    {
        Name = "BlogApp",
        HttpOnly = true,
        SameSite = SameSiteMode.Strict,
        SecurePolicy = isDevelopment ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always,
    };
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
await dbInitializer.InitializeAsync(scope.ServiceProvider, app.Lifetime.ApplicationStopping);
await dbInitializer.EnsurePostgreSqlSerilogTableAsync(builder.Configuration, app.Lifetime.ApplicationStopping);

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
