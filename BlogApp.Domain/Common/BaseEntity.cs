﻿using System;

namespace BlogApp.Domain.Common;

public abstract class BaseEntity : IEntityTimestamps
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CreatedById { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public int? UpdatedById { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
}
