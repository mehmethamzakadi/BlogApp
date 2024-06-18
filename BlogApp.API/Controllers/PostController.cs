using BlogApp.Application.Features.Posts.Commands.Create;
using BlogApp.Application.Features.Posts.Commands.Delete;
using BlogApp.Application.Features.Posts.Commands.Update;
using BlogApp.Application.Features.Posts.Queries.GetById;
using BlogApp.Application.Features.Posts.Queries.GetList;
using BlogApp.Domain.Common.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class PostController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
        {
            return GetResponse(await Mediator.Send(new GetListPostQuery(pageRequest)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return GetResponse(await Mediator.Send(new GetByIdPostQuery(id)));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreatePostCommand category)
        {
            return GetResponse(await Mediator.Send(category));
        }

        [HttpPut]
        public async Task<IActionResult> Put(UpdatePostCommand category)
        {
            return GetResponse(await Mediator.Send(category));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return GetResponse(await Mediator.Send(new DeletePostCommand(id)));
        }
    }
}