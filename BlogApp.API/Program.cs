using BlogApp.API.Options;
using BlogApp.Application;
using BlogApp.Infrastructure;
using BlogApp.Persistence;
using BlogApp.Persistence.DatabaseInitializer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
builder.Services.AddConfigurePersistenceServices(builder.Configuration);
builder.Services.AddConfigureApplicationServices(builder.Configuration);
builder.Services.AddConfigureInfrastructureServices(builder.Configuration);

builder.Services.AddOptions();

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder
                          .WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<TelegramOptions>(builder.Configuration.GetSection("TelegramBotOptions"));
builder.Services.Configure<TokenOptions>(builder.Configuration.GetSection("TokenOptions"));

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
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie = new CookieBuilder
    {
        Name = "BlogApp",
        HttpOnly = true,
        SameSite = SameSiteMode.Strict,
        SecurePolicy = CookieSecurePolicy.SameAsRequest //Canl�da Always olmal�d�r.,
    };
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Uygulama s�f�rdan docker ile aya�a kalkarken database migrate sa�lan�yor.
using (IServiceScope serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var myDependency = services.GetRequiredService<IDbInitializer>();

    //Veritaban� olu�turuluyor
    await myDependency.DatabaseInitializer(app, builder.Configuration);

    //Serilog i�in tablo yap�s� olu�turuluyor (MsSql)
    await myDependency.CreateMsSqlSeriLogTable(builder.Configuration);

    //Postgre i�in serilog
    //await myDependency.CreatePostgreSqlSeriLogTable(builder.Configuration);
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication(); //Kimlik do�rulamas�.

app.UseRouting();

app.UseAuthorization(); //Yetki kontrol�.

app.MapControllers();

app.Run();
