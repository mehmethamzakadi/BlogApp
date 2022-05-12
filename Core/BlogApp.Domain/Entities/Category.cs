using BlogApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApp.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
    }
}
