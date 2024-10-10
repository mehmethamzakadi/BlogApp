using BlogApp.Domain.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Domain.Common.Requests;

public class DataGridRequest
{
    public PaginatedRequest? PageRequest { get; set; }
    public DynamicQuery? DynamicQuery { get; set; }
}
