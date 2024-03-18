using BlogApp.Application.Features.Categories.Commands.Create;
using BlogApp.Application.Features.Categories.Commands.Delete;
using BlogApp.Application.Features.Categories.Commands.Update;
using BlogApp.Application.Features.Categories.Queries.GetById;
using BlogApp.Application.Features.Categories.Queries.GetList;
using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class CategoryController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
        {
            GetListResponse<GetListCategoryResponse> response = await Mediator.Send(new GetListCategoriesQuery(pageRequest));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetByIdCategoryQuery(id));
            return Ok(response);
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
            return GetResponseOnlyResultMessage(await Mediator.Send(new DeleteCategoryCommand(id)));
        }
    }
}