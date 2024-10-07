using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Seeds
{
    public class AppUserSeed : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            var user = new AppUser
            {
                Id = 1,
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                PhoneNumberConfirmed = false,
                LockoutEnabled = false,
                EmailConfirmed = false,
                TwoFactorEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var password = new PasswordHasher<AppUser>();
            var hashed = password.HashPassword(user, "mAdmin92");
            user.PasswordHash = hashed;

            builder.HasData(user);
        }
    }
}
