
namespace BlogApp.Application.Interfaces.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }
        Task Save();
    }
}
