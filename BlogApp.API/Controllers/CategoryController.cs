using BlogApp.Application.Features.Categories.Commands.Create;
using BlogApp.Application.Features.Categories.Commands.Delete;
using BlogApp.Application.Features.Categories.Commands.Update;
using BlogApp.Application.Features.Categories.Queries.GetById;
using BlogApp.Application.Features.Categories.Queries.GetList;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class CategoryController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return GetResponseOnlyResultData(await Mediator.Send(new GetListCategoriesQuery()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return GetResponseOnlyResultData(await Mediator.Send(new GetByIdCategoryQuery { Id = id }));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateCategoryCommand category)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(category));
        }

        [HttpPut]
        public async Task<IActionResult> Put(UpdateCategoryCommand category)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(category));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(new DeleteCategoryCommand { Id = id }));
        }
    }
}