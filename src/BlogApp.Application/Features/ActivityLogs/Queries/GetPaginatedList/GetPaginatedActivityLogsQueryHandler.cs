using AutoMapper;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Application.Features.ActivityLogs.Queries.GetPaginatedList;

public class GetPaginatedActivityLogsQueryHandler : IRequestHandler<GetPaginatedActivityLogsQuery, PaginatedListResponse<GetPaginatedActivityLogsResponse>>
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IMapper _mapper;

    public GetPaginatedActivityLogsQueryHandler(
        IActivityLogRepository activityLogRepository,
        IMapper mapper)
    {
        _activityLogRepository = activityLogRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedListResponse<GetPaginatedActivityLogsResponse>> Handle(
        GetPaginatedActivityLogsQuery request,
        CancellationToken cancellationToken)
    {
        Paginate<ActivityLog> activityLogs = await _activityLogRepository.GetPaginatedListAsync(
            orderBy: query => query.OrderByDescending(a => a.Timestamp),
            index: request.Request.PaginatedRequest.PageIndex,
            size: request.Request.PaginatedRequest.PageSize,
            include: a => a.Include(a => a.User!),
            cancellationToken: cancellationToken
        );

        PaginatedListResponse<GetPaginatedActivityLogsResponse> response = _mapper.Map<PaginatedListResponse<GetPaginatedActivityLogsResponse>>(activityLogs);
        return response;
    }
}
