using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories
{
    public class ImageRepository : EfRepositoryBase<Image, BlogAppDbContext>, IImageRepository
    {
        public ImageRepository(BlogAppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
