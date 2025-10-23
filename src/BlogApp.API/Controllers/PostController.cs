using BlogApp.Application.Features.Categories.Queries.GetPaginatedListByDynamic;
using BlogApp.Application.Features.Posts.Commands.Create;
using BlogApp.Application.Features.Posts.Commands.Delete;
using BlogApp.Application.Features.Posts.Commands.Update;
using BlogApp.Application.Features.Posts.Queries.GetById;
using BlogApp.Application.Features.Posts.Queries.GetPaginatedListByDynamic;
using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class PostController(IMediator mediator) : BaseApiController(mediator)
    {
        [HttpPost("GetPaginatedList")]
        public async Task<IActionResult> GetPaginatedListByDynamic(DataGridRequest dataGridRequest)
        {
            PaginatedListResponse<GetPaginatedListByDynamicPostsResponse> response = await Mediator.Send(new GetPaginatedListByDynamicPostsQuery(dataGridRequest));
            return Ok(response);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetByIdPostQuery(id));
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreatePostCommand category)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(category));
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdatePostCommand category)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(category));
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(new DeletePostCommand(id)));
        }
    }
}