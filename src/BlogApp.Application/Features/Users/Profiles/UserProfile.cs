using System.Collections.Generic;
using AutoMapper;
using BlogApp.Application.Features.Users.Commands.Create;
using BlogApp.Application.Features.Users.Queries.GetById;
using BlogApp.Application.Features.Users.Queries.GetList;
using BlogApp.Application.Features.Users.Queries.GetPaginatedListByDynamic;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Features.Users.Profiles;

public sealed class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, GetByIdUserResponse>().ReverseMap();
        CreateMap<User, GetListUserResponse>().ReverseMap();
        CreateMap<User, CreateUserCommand>().ReverseMap();

        CreateMap<Paginate<User>, PaginatedListResponse<GetListUserResponse>>().ReverseMap();

        CreateMap<UserRole, GetPaginatedListByDynamicUserRoleResponse>()
            .ConstructUsing(src => new GetPaginatedListByDynamicUserRoleResponse(
                src.RoleId,
                src.Role != null ? src.Role.Name : string.Empty
            ));

        CreateMap<User, GetPaginatedListByDynamicUsersResponse>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles ?? new List<UserRole>()))
            .ReverseMap()
            .ForMember(dest => dest.UserRoles, opt => opt.Ignore());
        CreateMap<Paginate<User>, PaginatedListResponse<GetPaginatedListByDynamicUsersResponse>>().ReverseMap();
    }
}
