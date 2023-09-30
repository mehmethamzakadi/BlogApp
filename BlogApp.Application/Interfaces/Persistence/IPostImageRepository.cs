using BlogApp.Application.Interfaces.Persistence.Common;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Interfaces.Persistence
{
    public interface IPostImageRepository : IAsyncRepository<PostImage>, IRepository<PostImage>
    {
    }
}
