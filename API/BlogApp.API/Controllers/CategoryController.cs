using BlogApp.Application.DTOs.Common;
using BlogApp.Application.DTOs.Params;
using BlogApp.Application.DTOs.Results;
using BlogApp.Application.Features.CategoryFeature.Commands;
using BlogApp.Application.Features.CategoryFeature.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class CategoryController : BaseApiController
    {
        [HttpGet]
        public async Task<BaseResult<IReadOnlyList<RsCategory>>> Get()
        {
            return await Mediator.Send(new GetAllCategoriesQuery());
        }

        [HttpGet("{id}")]
        public async Task<BaseResult<RsCategory>> Get(int id)
        {
            return await Mediator.Send(new GetByIdCategoryQuery { Id = id });
        }

        [HttpPost]
        public async Task<BaseResult<PmCategory>> Post(PmCategory category)
        {
            return await Mediator.Send(new CreateCategoryCommand { Name = category.Name });
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await Mediator.Send(new DeleteCategoryCommand { Id = id });
        }

        public async Task Put(RsCategory category)
        {
            await Mediator.Send(new UpdateCategoryCommand { Id = category.Id, Name = category.Name });
        }
    }
}