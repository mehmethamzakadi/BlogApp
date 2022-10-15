using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        private readonly BlogAppDbContext _dbContext;

        public PostRepository(BlogAppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
