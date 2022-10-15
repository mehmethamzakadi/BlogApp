using BlogApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApp.Domain.Entities
{
    public class Image : BaseEntity
    {
        public string Name { get; set; }
        public string? Title { get; set; }
        public decimal? Size { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
    }
}
