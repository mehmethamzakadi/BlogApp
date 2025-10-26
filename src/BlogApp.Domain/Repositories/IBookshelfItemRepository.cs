using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;

namespace BlogApp.Domain.Repositories;

public interface IBookshelfItemRepository : IAsyncRepository<BookshelfItem>, IRepository<BookshelfItem>
{
}
