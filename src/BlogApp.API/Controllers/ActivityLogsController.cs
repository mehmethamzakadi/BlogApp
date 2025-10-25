using BlogApp.Application.Features.ActivityLogs.Queries.GetPaginatedList;
using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Constants;
using BlogApp.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers;

public class ActivityLogsController(IMediator mediator) : BaseApiController(mediator)
{
    /// <summary>
    /// Activity log'ları paginated ve filtrelenmiş şekilde getirir
    /// </summary>
    [HttpPost("search")]
    [HasPermission(Permissions.DashboardView)]
    public async Task<IActionResult> GetPaginatedList([FromBody] DataGridRequest request)
    {
        PaginatedListResponse<GetPaginatedActivityLogsResponse> response = 
            await Mediator.Send(new GetPaginatedActivityLogsQuery(request));
        return Ok(response);
    }
}
