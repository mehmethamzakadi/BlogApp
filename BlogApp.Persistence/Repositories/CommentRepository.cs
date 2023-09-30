using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories
{
    public class CommentRepository : EfRepositoryBase<Comment, BlogAppDbContext>, ICommentRepository
    {
        public CommentRepository(BlogAppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
