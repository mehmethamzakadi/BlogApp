﻿using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;

namespace BlogApp.Domain.Repositories;

public interface ICategoryRepository : IAsyncRepository<Category>, IRepository<Category>
{
}
