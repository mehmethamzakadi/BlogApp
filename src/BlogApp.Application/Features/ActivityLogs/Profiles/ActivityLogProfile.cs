using AutoMapper;
using BlogApp.Application.Features.ActivityLogs.Queries.GetPaginatedList;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Features.ActivityLogs.Profiles;

public sealed class ActivityLogProfile : Profile
{
    public ActivityLogProfile()
    {
        CreateMap<ActivityLog, GetPaginatedActivityLogsResponse>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty));

        CreateMap<Paginate<ActivityLog>, PaginatedListResponse<GetPaginatedActivityLogsResponse>>();
    }
}
