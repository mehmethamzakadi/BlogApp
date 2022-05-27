using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;

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
