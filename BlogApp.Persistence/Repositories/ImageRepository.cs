using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;

namespace BlogApp.Persistence.Repositories;

public class ImageRepository(BlogAppDbContext dbContext) : EfRepositoryBase<Image, BlogAppDbContext>(dbContext), IImageRepository
{
}
