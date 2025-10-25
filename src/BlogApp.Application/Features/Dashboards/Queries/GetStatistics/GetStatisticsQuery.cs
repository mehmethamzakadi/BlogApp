using MediatR;

namespace BlogApp.Application.Features.Dashboards.Queries.GetStatistics;

public sealed record GetStatisticsQuery : IRequest<GetStatisticsResponse>;
