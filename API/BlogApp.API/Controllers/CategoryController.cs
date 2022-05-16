using BlogApp.Application.DTOs.Category;
using BlogApp.Application.Features.Category.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class CategoryController : BaseApiController
    {
        [HttpGet]
        public async Task<IReadOnlyList<RsCategory>> Get()
        {
            return await Mediator.Send(new GetAllCategoriesQuery());
        }

        [HttpGet("{id}")]
        public async Task<RsCategory> Get(int id)
        {
            return await Mediator.Send(new GetByIdCategoryQuery { Id = id });
        }
    }
}