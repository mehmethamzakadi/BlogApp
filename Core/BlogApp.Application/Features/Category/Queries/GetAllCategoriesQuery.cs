using BlogApp.Application.DTOs.Category;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Application.Features.Category.Queries
{
    public class GetAllCategoriesQuery : IRequest<IReadOnlyList<RsCategory>>
    {

    }
}
