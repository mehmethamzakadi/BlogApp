using BlogApp.Application.Features.Posts.Commands.Create;
using BlogApp.Application.Features.Posts.Commands.Delete;
using BlogApp.Application.Features.Posts.Commands.Update;
using BlogApp.Application.Features.Posts.Queries.GetById;
using BlogApp.Application.Features.Posts.Queries.GetList;
using BlogApp.Application.Features.Posts.Queries.GetListByCategoryId;
using BlogApp.Application.Features.Posts.Queries.GetPaginatedListByDynamic;
using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class PostController(IMediator mediator) : BaseApiController(mediator)
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PaginatedRequest pageRequest)
        {
            PaginatedListResponse<GetListPostResponse> response = await Mediator.Send(new GetListPostQuery(pageRequest));
            return Ok(response);
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] DataGridRequest dataGridRequest)
        {
            PaginatedListResponse<GetPaginatedListByDynamicPostsResponse> response = await Mediator.Send(new GetPaginatedListByDynamicPostsQuery(dataGridRequest));
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            bool includeUnpublished = User.Identity?.IsAuthenticated == true;
            var response = await Mediator.Send(new GetByIdPostQuery(id, includeUnpublished));
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePostCommand command)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(command));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePostCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");
            
            return GetResponseOnlyResultMessage(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(new DeletePostCommand(id)));
        }
    }
}
