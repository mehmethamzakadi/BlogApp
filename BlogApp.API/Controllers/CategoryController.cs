using BlogApp.Application.Features.Categories.Commands.Create;
using BlogApp.Application.Features.Categories.Commands.Delete;
using BlogApp.Application.Features.Categories.Commands.Update;
using BlogApp.Application.Features.Categories.Queries.GetById;
using BlogApp.Application.Features.Categories.Queries.GetList;
using BlogApp.Domain.Common.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class CategoryController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
        {
            return GetResponse(await Mediator.Send(new GetListCategoriesQuery(pageRequest)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return GetResponse(await Mediator.Send(new GetByIdCategoryQuery(id)));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateCategoryCommand category)
        {
            return GetResponse(await Mediator.Send(category));
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateCategoryCommand category)
        {
            return GetResponse(await Mediator.Send(category));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return GetResponse(await Mediator.Send(new DeleteCategoryCommand(id)));
        }
    }
}