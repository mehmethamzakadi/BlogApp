using BlogApp.Application.Features.Posts.Commands.Create;
using BlogApp.Application.Features.Posts.Commands.Delete;
using BlogApp.Application.Features.Posts.Commands.Update;
using BlogApp.Application.Features.Posts.Queries.GetById;
using BlogApp.Application.Features.Posts.Queries.GetList;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class PostController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return GetResponseOnlyResultData(await Mediator.Send(new GetListPostQuery()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return GetResponseOnlyResultData(await Mediator.Send(new GetByIdPostQuery { Id = id }));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreatePostCommand category)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(category));
        }

        [HttpPut]
        public async Task<IActionResult> Put(UpdatePostCommand category)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(category));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(new DeletePostCommand { Id = id }));
        }
    }
}