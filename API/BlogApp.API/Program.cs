using BlogApp.Application;
using BlogApp.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddConfigurePersistenceServices(builder.Configuration);
builder.Services.AddConfigureApplicationServices();

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddSession();
//builder.Services.AddDistributedMemoryCache();

//builder.Services.ConfigureApplicationCookie(opt =>
//{
//    opt.LoginPath = new PathString("/Admin/User/Login");
//    opt.LogoutPath = new PathString("/Admin/User/Logout");
//    opt.Cookie = new CookieBuilder
//    {
//        Name = "BlogApp",
//        HttpOnly = true,
//        SameSite = SameSiteMode.Strict,
//        SecurePolicy = CookieSecurePolicy.SameAsRequest //Canlýda Always olmalýdýr.,
//    };
//    opt.SlidingExpiration = true;
//    opt.ExpireTimeSpan = TimeSpan.FromDays(7);
//    opt.AccessDeniedPath = new PathString("/Admin/User/AccessDenied");
//});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseSession();

app.UseHttpsRedirection();

app.UseAuthentication(); //Kimlik doðrulamasý.

app.UseAuthorization(); //Yetki kontrolü.

app.MapControllers();

app.Run();
