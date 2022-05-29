using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Persistence.Contexts
{
    public class BlogAppDbContext : AuditableDbContext
    {
        public BlogAppDbContext(DbContextOptions<BlogAppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options, httpContextAccessor)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlogAppDbContext).Assembly);
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AppUserToken> AppUserTokens { get; set; }


    }
}
