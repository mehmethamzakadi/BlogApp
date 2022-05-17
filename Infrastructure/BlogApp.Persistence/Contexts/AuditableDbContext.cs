using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Persistence.Contexts
{
    public class AuditableDbContext : IdentityDbContext<AppUser, AppRole, int, AppUserClaim, AppUserRole, AppUserLogin, AppRoleClaim, AppUserToken>
    {
        public AuditableDbContext(DbContextOptions options) : base(options)
        {
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in base.ChangeTracker.Entries<BaseEntity>()
               .Where(q => q.State == EntityState.Added || q.State == EntityState.Modified || q.State == EntityState.Deleted))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedById = 1; //Geçici olarak 1 verildi. Düzenlenecek.
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                    entry.Entity.UpdatedById = 1; //Geçici olarak 1 verildi. Düzenlenecek.
                }

                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedById = 1; //Geçici olarak 1 verildi. Düzenlenecek.
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
    }
}
