using BlogApp.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogApp.Persistence.Contexts
{
    public class AuditableDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuditableDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userIdClaim = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = int.TryParse(userIdClaim, out var parsedUserId) ? parsedUserId : 0;

            // Eğer kullanıcı yoksa (seed işlemleri için), system kullanıcısı olarak işaretle
            // Gerçek API isteklerinde userId mutlaka olmalı
            var isSystemOperation = userId == 0 && _httpContextAccessor?.HttpContext == null;
            var effectiveUserId = isSystemOperation ? 1 : userId; // System operations için ID 1 kullan

            foreach (var entry in base.ChangeTracker.Entries<BaseEntity>()
               .Where(q => q.State == EntityState.Added || q.State == EntityState.Modified || q.State == EntityState.Deleted))
            {
                if (entry.State == EntityState.Added)
                {
                    // Sadece gerçek API isteklerinde (HttpContext varsa) kullanıcı kontrolü yap
                    if (!isSystemOperation && userId == 0)
                    {
                        throw new UnauthorizedAccessException("Kullanıcı bilgisi bulunamadı. Lütfen giriş yapınız.");
                    }

                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedById = effectiveUserId;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                    entry.Entity.UpdatedById = effectiveUserId;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
    }
}
