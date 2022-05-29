using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogAppDbContext _dbContext;
        private ICategoryRepository _categoryRepository;
        private IAppUserTokenRepository _appUserTokenRepository;

        public UnitOfWork(BlogAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ICategoryRepository CategoryRepository =>
           _categoryRepository ??= new CategoryRepository(_dbContext);

        public IAppUserTokenRepository AppUserTokenRepository =>
           _appUserTokenRepository ??= new AppUserTokenRepository(_dbContext);


        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
