
namespace BlogApp.Application.Interfaces.Persistence
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IPostRepository PostRepository { get; }
        IImageRepository ImageRepository { get; }
        ICommentRepository CommentRepository { get; }
        IPostImageRepository PostImageRepository { get; }
        IPostCategoryRepository PostCategoryRepository { get; }
        IAppUserTokenRepository AppUserTokenRepository { get; }
        void SaveChanges();
        Task SaveChangesAsync();
        void Rollback();
        Task RollbackAsync();
    }
}
