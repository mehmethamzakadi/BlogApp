using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
