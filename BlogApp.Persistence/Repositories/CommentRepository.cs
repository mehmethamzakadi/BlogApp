using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories;

public class CommentRepository : EfRepositoryBase<Comment, BlogAppDbContext>, ICommentRepository
{
    public CommentRepository(BlogAppDbContext dbContext) : base(dbContext)
    {
    }
}
