using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories
{
    public class CategoryRepository : EfRepositoryBase<Category, BlogAppDbContext>, ICategoryRepository
    {

        public CategoryRepository(BlogAppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
