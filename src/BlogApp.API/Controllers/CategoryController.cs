using BlogApp.Application.Features.Categories.Commands.Create;
using BlogApp.Application.Features.Categories.Commands.Delete;
using BlogApp.Application.Features.Categories.Commands.Update;
using BlogApp.Application.Features.Categories.Queries.GetAll;
using BlogApp.Application.Features.Categories.Queries.GetById;
using BlogApp.Application.Features.Categories.Queries.GetPaginatedListByDynamic;
using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class CategoryController(IMediator mediator) : BaseApiController(mediator)
    {
        [HttpPost("GetPaginatedList")]
        public async Task<IActionResult> GetPaginatedListByDynamic(DataGridRequest dataGridRequest)
        {
            PaginatedListResponse<GetPaginatedListByDynamicCategoriesResponse> response = await Mediator.Send(new GetPaginatedListByDynamicCategoriesQuery(dataGridRequest));
            return Ok(response);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            IQueryable response = await Mediator.Send(new GetAllListCategoriesQuery());
            return Ok(response);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetByIdCategoryQuery(id));
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateCategoryCommand category)
        {
            var response = await Mediator.Send(category);
            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryCommand category)
        {
            var response = await Mediator.Send(category);
            return Ok(response);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await Mediator.Send(new DeleteCategoryCommand(id));
            return Ok(response);
        }
    }
}