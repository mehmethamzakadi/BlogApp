using AspNetCoreRateLimit;
using BlogApp.API.Configuration;
using BlogApp.API.Filters;
using BlogApp.API.Middlewares;
using BlogApp.Application;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Options;
using BlogApp.Infrastructure;
using BlogApp.Persistence;
using BlogApp.Persistence.DatabaseInitializer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog yapılandırmasını yükle
builder.ConfigureSerilog();

var corsPolicyName = "_dynamicCorsPolicy";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
if (allowedOrigins.Length == 0)
{
    throw new InvalidOperationException("En az bir izinli CORS origin yapılandırılmalıdır (Cors:AllowedOrigins).");
}

builder.Services.AddConfigurePersistenceServices(builder.Configuration);
builder.Services.AddConfigureApplicationServices(builder.Configuration);
builder.Services.AddConfigureInfrastructureServices(builder.Configuration);

builder.Services.AddOptions();
builder.Services.AddHttpContextAccessor();

// Rate Limit yapılandırması
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName, policyBuilder =>
    {
        policyBuilder.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // React withCredentials için gerekli
    });
});

builder.Services.AddControllers(options =>
    {
        // Request/Response loglama için global action filter ekle
        options.Filters.Add<RequestResponseLoggingFilter>();
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(state => state.Value?.Errors.Count > 0)
                .SelectMany(state => state.Value!.Errors)
                .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? "Geçersiz veya eksik bilgiler mevcut."
                    : error.ErrorMessage)
                .Distinct()
                .ToList();

            var apiResult = new ApiResult<object>
            {
                Success = false,
                Message = errors.FirstOrDefault() ?? "Geçersiz veya eksik bilgiler mevcut.",
                InternalMessage = "ModelValidationError",
                Errors = errors
            };

            return new BadRequestObjectResult(apiResult);
        };
    });

// Küçük harfli URL'ler için routing yapılandırması
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = false;
});
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
        SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.Strict,
        SecurePolicy = isDevelopment ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always,
    };
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

var app = builder.Build();

app.UseStaticFiles();

var imageStorageOptions = app.Services.GetRequiredService<IOptions<ImageStorageOptions>>().Value;
var imageRootPath = imageStorageOptions.RootPath;
if (!Path.IsPathRooted(imageRootPath))
{
    imageRootPath = Path.Combine(app.Environment.ContentRootPath, imageRootPath);
}

imageRootPath = Path.GetFullPath(imageRootPath);
Directory.CreateDirectory(imageRootPath);

if (Directory.Exists(imageRootPath))
{
    var requestPathValue = (imageStorageOptions.RequestPath ?? string.Empty).Trim();
    if (!string.IsNullOrWhiteSpace(requestPathValue) && !requestPathValue.StartsWith('/'))
    {
        requestPathValue = "/" + requestPathValue;
    }

    requestPathValue = requestPathValue.TrimEnd('/');

    var staticFileOptions = new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(imageRootPath),
        RequestPath = string.IsNullOrWhiteSpace(requestPathValue) ? default : new PathString(requestPathValue)
    };

    app.UseStaticFiles(staticFileOptions);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();  // OpenAPI dokümanı için endpoint ekle

    app.MapScalarApiReference(options =>
    {
        options.Title = "BlogApp API";
        options.Theme = ScalarTheme.DeepSpace; // DeepSpace, Light, Solar gibi temalar kullanılabilir
    });
}

// Serilog request logging ekle
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());

        if (httpContext.User?.Identity?.IsAuthenticated == true)
        {
            diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
        }
    };
});

// Veritabanı başlatma ve gerekli tabloları oluştur
await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
await dbInitializer.InitializeAsync(scope.ServiceProvider, app.Lifetime.ApplicationStopping);
await dbInitializer.EnsurePostgreSqlSerilogTableAsync(builder.Configuration, app.Lifetime.ApplicationStopping);

//app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();

app.UseCors(corsPolicyName); // CORS, endpoint routing sonrası ve auth öncesi konumlandırılmalı

app.UseIpRateLimiting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

