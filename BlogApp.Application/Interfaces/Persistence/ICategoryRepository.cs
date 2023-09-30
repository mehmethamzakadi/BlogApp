using BlogApp.Application.Interfaces.Persistence.Common;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Interfaces.Persistence
{
    public interface ICategoryRepository : IAsyncRepository<Category>, IRepository<Category>
    {
    }
}
