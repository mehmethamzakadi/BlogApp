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

        CreateMap<User, GetPaginatedListByDynamicUsersResponse>().ReverseMap();
        CreateMap<Paginate<User>, PaginatedListResponse<GetPaginatedListByDynamicUsersResponse>>().ReverseMap();
    }
}
