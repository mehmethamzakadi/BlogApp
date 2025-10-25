using BlogApp.Domain.Repositories;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlogApp.Application.Features.Dashboards.Queries.GetRecentActivities;

public sealed class GetRecentActivitiesQueryHandler : IRequestHandler<GetRecentActivitiesQuery, GetRecentActivitiesResponse>
{
    private readonly IActivityLogRepository _activityLogRepository;

    public GetRecentActivitiesQueryHandler(IActivityLogRepository activityLogRepository)
    {
        _activityLogRepository = activityLogRepository;
    }

    public async Task<GetRecentActivitiesResponse> Handle(GetRecentActivitiesQuery request, CancellationToken cancellationToken)
    {
        var activities = await _activityLogRepository.GetRecentActivitiesAsync(request.Count, cancellationToken);

        var activityDtos = activities.Select(a => new ActivityDto
        {
            Id = a.Id,
            ActivityType = a.ActivityType,
            EntityType = a.EntityType,
            EntityId = a.EntityId,
            Title = a.Title,
            Timestamp = a.Timestamp,
            UserName = a.User?.UserName
        }).ToList();

        return new GetRecentActivitiesResponse
        {
            Activities = activityDtos
        };
    }
}
