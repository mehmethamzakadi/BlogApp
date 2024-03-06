using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories;

public class PostRepository : EfRepositoryBase<Post, BlogAppDbContext>, IPostRepository
{
    public PostRepository(BlogAppDbContext dbContext) : base(dbContext)
    {
    }
}
