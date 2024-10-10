using AutoMapper;
using BlogApp.Application.Features.AppRoles.Queries.GetList;
using BlogApp.Application.Features.AppRoles.Queries.GetRoleById;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Features.AppRoles.Profiles
{
    public class AppRoleProfile : Profile
    {
        public AppRoleProfile()
        {
            CreateMap<AppRole, GetListAppRoleResponse>().ReverseMap();
            CreateMap<AppRole, GetRoleByIdQueryResponse>().ReverseMap();

            CreateMap<Paginate<AppRole>, PaginatedListResponse<GetListAppRoleResponse>>().ReverseMap();
        }
    }
}
