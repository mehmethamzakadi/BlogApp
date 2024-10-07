using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories;

public class CommentRepository(BlogAppDbContext dbContext) : EfRepositoryBase<Comment, BlogAppDbContext>(dbContext), ICommentRepository
{
}
