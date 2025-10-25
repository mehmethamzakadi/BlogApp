using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories;

public class CategoryRepository(BlogAppDbContext dbContext) : EfRepositoryBase<Category, BlogAppDbContext>(dbContext), ICategoryRepository
{
}
