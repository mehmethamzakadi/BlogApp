using BlogApp.Application.Features.BookshelfItems.Commands.Create;
using BlogApp.Application.Features.BookshelfItems.Commands.Delete;
using BlogApp.Application.Features.BookshelfItems.Commands.Update;
using BlogApp.Application.Features.BookshelfItems.Queries.GetById;
using BlogApp.Application.Features.BookshelfItems.Queries.GetPaginatedListByDynamic;
using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Constants;
using BlogApp.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers;

public class BookshelfController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpPost("search")]
    [HasPermission(Permissions.BookshelfViewAll)]
    public async Task<IActionResult> Search([FromBody] DataGridRequest dataGridRequest)
    {
        PaginatedListResponse<GetPaginatedListByDynamicBookshelfItemsResponse> response = await Mediator.Send(new GetPaginatedListByDynamicBookshelfItemsQuery(dataGridRequest));
        return Ok(response);
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.BookshelfRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var response = await Mediator.Send(new GetByIdBookshelfItemQuery(id));
        return GetResponseOnlyResultData(response);
    }

    [HttpPost]
    [HasPermission(Permissions.BookshelfCreate)]
    public async Task<IActionResult> Create([FromBody] CreateBookshelfItemCommand command)
    {
        return GetResponseOnlyResultMessage(await Mediator.Send(command));
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.BookshelfUpdate)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBookshelfItemCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        return GetResponseOnlyResultMessage(await Mediator.Send(command));
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.BookshelfDelete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return GetResponseOnlyResultMessage(await Mediator.Send(new DeleteBookshelfItemCommand(id)));
    }
}
