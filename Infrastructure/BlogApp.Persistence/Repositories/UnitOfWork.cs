using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogAppDbContext _dbContext;
        private ICategoryRepository _categoryRepository;

        public UnitOfWork(BlogAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ICategoryRepository CategoryRepository =>
           _categoryRepository ??= new CategoryRepository(_dbContext);


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
