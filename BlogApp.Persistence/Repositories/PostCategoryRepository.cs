using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories
{
    public class PostCategoryRepository : EfRepositoryBase<PostCategory, BlogAppDbContext>, IPostCategoryRepository
    {
        public PostCategoryRepository(BlogAppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
