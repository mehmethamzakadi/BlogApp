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
        [HttpGet("GetList")]
        public async Task<IActionResult> GetList([FromQuery] PaginatedRequest pageRequest)
        {
            PaginatedListResponse<GetListPostResponse> response = await Mediator.Send(new GetListPostQuery(pageRequest));
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("GetListByCategoryId")]
        public async Task<IActionResult> GetListByCategoryId([FromQuery] PaginatedRequest pageRequest, [FromQuery] int categoryId)
        {
            PaginatedListResponse<GetListPostByCategoryIdResponse> response = await Mediator.Send(new GetListPostByCategoryIdQuery(pageRequest, categoryId));
            return Ok(response);
        }

        [HttpPost("GetPaginatedList")]
        public async Task<IActionResult> GetPaginatedListByDynamic(DataGridRequest dataGridRequest)
        {
            PaginatedListResponse<GetPaginatedListByDynamicPostsResponse> response = await Mediator.Send(new GetPaginatedListByDynamicPostsQuery(dataGridRequest));
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            bool includeUnpublished = User.Identity?.IsAuthenticated == true;
            var response = await Mediator.Send(new GetByIdPostQuery(id, includeUnpublished));
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
