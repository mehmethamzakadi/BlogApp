using BlogApp.Application.Features.Dashboards.Queries.GetRecentActivities;
using BlogApp.Application.Features.Dashboards.Queries.GetStatistics;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    public class DashboardController(IMediator mediator) : BaseApiController(mediator)
    {
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            GetStatisticsResponse response = await Mediator.Send(new GetStatisticsQuery());
            return Ok(response);
        }

        [HttpGet("activities")]
        public async Task<IActionResult> GetRecentActivities([FromQuery] int count = 10)
        {
            GetRecentActivitiesResponse response = await Mediator.Send(new GetRecentActivitiesQuery(count));
            return Ok(response);
        }
    }
}
