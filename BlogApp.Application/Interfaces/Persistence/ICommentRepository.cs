using BlogApp.Domain.Entities;

namespace BlogApp.Application.Interfaces.Persistence
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
    }
}
