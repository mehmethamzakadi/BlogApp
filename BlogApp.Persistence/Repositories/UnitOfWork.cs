using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogAppDbContext _dbContext;
        private ICategoryRepository _categoryRepository;
        private IPostRepository _postRepository;
        private ICommentRepository _commentRepository;
        private IImageRepository _imageRepository;
        private IPostImageRepository _postImageRepository;
        private IPostCategoryRepository _postCategoryRepository;
        private IAppUserTokenRepository _appUserTokenRepository;

        public UnitOfWork(BlogAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ICategoryRepository CategoryRepository =>
           _categoryRepository ??= new CategoryRepository(_dbContext);

        public IPostRepository PostRepository =>
          _postRepository ??= new PostRepository(_dbContext);

        public ICommentRepository CommentRepository =>
          _commentRepository ??= new CommentRepository(_dbContext);

        public IImageRepository ImageRepository =>
          _imageRepository ??= new ImageRepository(_dbContext);

        public IPostImageRepository PostImageRepository =>
         _postImageRepository ??= new PostImageRepository(_dbContext);

        public IPostCategoryRepository PostCategoryRepository =>
         _postCategoryRepository ??= new PostCategoryRepository(_dbContext);

        public IAppUserTokenRepository AppUserTokenRepository =>
           _appUserTokenRepository ??= new AppUserTokenRepository(_dbContext);


        public void Rollback()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task RollbackAsync()
        {
            await _dbContext.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
