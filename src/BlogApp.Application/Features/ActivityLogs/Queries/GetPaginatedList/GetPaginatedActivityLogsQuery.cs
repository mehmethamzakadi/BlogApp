using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;

namespace BlogApp.Application.Features.ActivityLogs.Queries.GetPaginatedList;

public class GetPaginatedActivityLogsQuery : IRequest<PaginatedListResponse<GetPaginatedActivityLogsResponse>>
{
    public DataGridRequest Request { get; set; }

    public GetPaginatedActivityLogsQuery(DataGridRequest request)
    {
        Request = request;
    }
}
