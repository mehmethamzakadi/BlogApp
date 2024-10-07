using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Persistence.Contexts
{
    public class AuditableDbContext : IdentityDbContext<AppUser, AppRole, int, AppUserClaim, AppUserRole, AppUserLogin, AppRoleClaim, AppUserToken>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuditableDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = Convert.ToInt32(_httpContextAccessor?.HttpContext?.User?.FindFirst("Id")?.Value);
            foreach (var entry in base.ChangeTracker.Entries<BaseEntity>()
               .Where(q => q.State == EntityState.Added || q.State == EntityState.Modified || q.State == EntityState.Deleted))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedById = userId;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                    entry.Entity.UpdatedById = userId;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
    }
}
