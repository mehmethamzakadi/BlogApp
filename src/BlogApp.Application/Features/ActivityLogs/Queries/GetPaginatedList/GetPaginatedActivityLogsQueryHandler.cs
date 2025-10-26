using AutoMapper;
using BlogApp.Domain.Common.Dynamic;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

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
        DynamicQuery dynamicQuery = request.Request.DynamicQuery ?? new DynamicQuery();

        List<Sort> sortDescriptors = dynamicQuery.Sort?.ToList() ?? new List<Sort>();
        if (sortDescriptors.Count == 0)
        {
            sortDescriptors.Add(new Sort(nameof(ActivityLog.Timestamp), "desc"));
        }

        dynamicQuery.Sort = sortDescriptors;

        Paginate<ActivityLog> activityLogs = await _activityLogRepository.GetPaginatedListByDynamicAsync(
            dynamic: dynamicQuery,
            index: request.Request.PaginatedRequest.PageIndex,
            size: request.Request.PaginatedRequest.PageSize,
            include: a => a.Include(a => a.User!),
            cancellationToken: cancellationToken
        );

        PaginatedListResponse<GetPaginatedActivityLogsResponse> response = _mapper.Map<PaginatedListResponse<GetPaginatedActivityLogsResponse>>(activityLogs);
        return response;
    }
}
