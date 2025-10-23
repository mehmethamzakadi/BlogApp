
using BlogApp.Application;
using BlogApp.Infrastructure;
using BlogApp.Persistence;
using BlogApp.Persistence.DatabaseInitializer;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var corsPolicyName = "_dynamicCorsPolicy";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
if (allowedOrigins.Length == 0)
{
    throw new InvalidOperationException("At least one allowed CORS origin must be configured under Cors:AllowedOrigins.");
}

builder.Services.AddConfigurePersistenceServices(builder.Configuration);
builder.Services.AddConfigureApplicationServices(builder.Configuration);
builder.Services.AddConfigureInfrastructureServices(builder.Configuration);

builder.Services.AddOptions();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName, policyBuilder =>
    {
        policyBuilder.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
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
    app.MapOpenApi();  // OpenAPI dokümaný için endpoint

    app.MapScalarApiReference(options =>
    {
        options.Title = "BlogApp API";
        options.Theme = ScalarTheme.DeepSpace; // Ýstersen Light, Solar, DeepSpace vs.
    });

}

await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
await dbInitializer.InitializeAsync(scope.ServiceProvider, app.Lifetime.ApplicationStopping);
await dbInitializer.EnsurePostgreSqlSerilogTableAsync(builder.Configuration, app.Lifetime.ApplicationStopping);

//app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(corsPolicyName);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
