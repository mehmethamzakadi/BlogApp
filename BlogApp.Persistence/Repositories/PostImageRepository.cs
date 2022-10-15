using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories
{
    public class PostImageRepository : GenericRepository<PostImage>, IPostImageRepository
    {
        private readonly BlogAppDbContext _dbContext;

        public PostImageRepository(BlogAppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
