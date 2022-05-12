using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Persistence.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly BlogAppDbContext _dbContext;

        public CategoryRepository(BlogAppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
