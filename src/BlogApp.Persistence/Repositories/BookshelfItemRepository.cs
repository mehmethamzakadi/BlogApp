using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories;

public class BookshelfItemRepository(BlogAppDbContext dbContext) : EfRepositoryBase<BookshelfItem, BlogAppDbContext>(dbContext), IBookshelfItemRepository
{
}
