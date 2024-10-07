using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;

namespace BlogApp.Domain.Repositories;

public interface IImageRepository : IAsyncRepository<Image>, IRepository<Image>
{
}
