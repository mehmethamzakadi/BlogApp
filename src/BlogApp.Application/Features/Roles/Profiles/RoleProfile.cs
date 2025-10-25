using AutoMapper;
using BlogApp.Application.Features.Roles.Queries.GetList;
using BlogApp.Application.Features.Roles.Queries.GetRoleById;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Features.Roles.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, GetListRoleResponse>().ReverseMap();
            CreateMap<Role, GetRoleByIdQueryResponse>().ReverseMap();

            CreateMap<Paginate<Role>, PaginatedListResponse<GetListRoleResponse>>().ReverseMap();
        }
    }
}
