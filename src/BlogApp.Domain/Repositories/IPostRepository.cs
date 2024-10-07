using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;

namespace BlogApp.Domain.Repositories;

public interface IPostRepository : IAsyncRepository<Post>, IRepository<Post>
{
}
