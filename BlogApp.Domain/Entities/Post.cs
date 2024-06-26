﻿using BlogApp.Domain.Common;
using System.Collections.Generic;

namespace BlogApp.Domain.Entities;

public sealed class Post : BaseEntity
{
    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public string Thumbnail { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = default!;

}
