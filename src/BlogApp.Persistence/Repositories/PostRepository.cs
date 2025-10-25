using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories;

public class PostRepository(BlogAppDbContext dbContext) : EfRepositoryBase<Post, BlogAppDbContext>(dbContext), IPostRepository
{
}
