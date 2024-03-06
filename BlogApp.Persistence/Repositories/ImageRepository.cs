using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories;

public class ImageRepository : EfRepositoryBase<Image, BlogAppDbContext>, IImageRepository
{
    public ImageRepository(BlogAppDbContext dbContext) : base(dbContext)
    {
    }
}
