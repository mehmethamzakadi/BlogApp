using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories
{
    public class PostImageRepository : EfRepositoryBase<PostImage, BlogAppDbContext>, IPostImageRepository
    {
        public PostImageRepository(BlogAppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
