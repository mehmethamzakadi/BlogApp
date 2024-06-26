﻿using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;

namespace BlogApp.Domain.Repositories;

public interface ICommentRepository : IAsyncRepository<Comment>, IRepository<Comment>
{
}
