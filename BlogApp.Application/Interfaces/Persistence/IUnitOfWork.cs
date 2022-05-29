
namespace BlogApp.Application.Interfaces.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }
        IAppUserTokenRepository AppUserTokenRepository { get; }
        Task SaveAsync();
    }
}
