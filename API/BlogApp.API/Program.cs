using BlogApp.Application;
using BlogApp.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddConfigureApplicationServices();
builder.Services.AddConfigurePersistenceServices(builder.Configuration);
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = new PathString("/Admin/User/Login");
    opt.LogoutPath = new PathString("/Admin/User/Logout");
    opt.Cookie = new CookieBuilder
    {
        Name = "BlogApp",
        HttpOnly = true,
        SameSite = SameSiteMode.Strict,
        SecurePolicy = CookieSecurePolicy.SameAsRequest //Canlýda Always olmalýdýr.,
    };
    opt.SlidingExpiration = true;
    opt.ExpireTimeSpan = TimeSpan.FromDays(7);
    opt.AccessDeniedPath = new PathString("/Admin/User/AccessDenied");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication(); //Kimlik doðrulamasý.

app.UseAuthorization(); //Yetki kontrolü.

app.MapControllers();

app.Run();
