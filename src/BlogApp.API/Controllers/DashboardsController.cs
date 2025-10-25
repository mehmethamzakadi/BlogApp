using BlogApp.Application.Features.Dashboards.Queries.GetStatistics;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class DashboardsController(IMediator mediator) : BaseApiController(mediator)
    {
        [HttpGet("GetStatistics")]
        public async Task<IActionResult> GetStatistics()
        {
            GetStatisticsResponse response = await Mediator.Send(new GetStatisticsQuery());
            return Ok(response);
        }
    }
}
