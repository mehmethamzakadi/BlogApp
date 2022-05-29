using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories
{
    public class AppUserTokenRepository : GenericRepository<AppUserToken>, IAppUserTokenRepository
    {
        private readonly BlogAppDbContext _dbContext;

        public AppUserTokenRepository(BlogAppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
