using BlogApp.Application.DTOs;
using BlogApp.Application.DTOs.Common;
using BlogApp.Application.Features.Categories.Commands;
using BlogApp.Application.Features.Categories.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class CategoryController : BaseApiController
    {
        [HttpGet]
        public async Task<BaseResult<IReadOnlyList<CategoryDto>>> Get()
        {
            return await Mediator.Send(new GetAllCategoriesQuery());
        }

        [HttpGet("{id}")]
        public async Task<BaseResult<CategoryDto>> Get(int id)
        {
            return await Mediator.Send(new GetByIdCategoryQuery { Id = id });
        }

        [HttpPost]
        public async Task<BaseResult<CreateCategoryCommand>> Post(CreateCategoryCommand category)
        {
            return await Mediator.Send(new CreateCategoryCommand { Name = category.Name });
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await Mediator.Send(new DeleteCategoryCommand { Id = id });
        }

        [HttpPut]
        public async Task Put(UpdateCategoryCommand category)
        {
            await Mediator.Send(new UpdateCategoryCommand { Id = category.Id, Name = category.Name });
        }
    }
}