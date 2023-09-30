using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories
{
    public class PostRepository : EfRepositoryBase<Post, BlogAppDbContext>, IPostRepository
    {
        public PostRepository(BlogAppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
