using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories;

public class CategoryRepository : EfRepositoryBase<Category, BlogAppDbContext>, ICategoryRepository
{

    public CategoryRepository(BlogAppDbContext dbContext) : base(dbContext)
    {
    }
}
