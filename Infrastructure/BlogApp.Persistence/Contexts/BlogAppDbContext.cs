using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Persistence.Contexts
{
    public class BlogAppDbContext : AuditableDbContext
    {
        public BlogAppDbContext(DbContextOptions<BlogAppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlogAppDbContext).Assembly);
        }

        public DbSet<Category> Categories { get; set; }
    }
}
