using MediatR;

namespace BlogApp.Application.Features.Dashboards.Queries.GetRecentActivities;

public sealed record GetRecentActivitiesQuery(int Count = 10) : IRequest<GetRecentActivitiesResponse>;
