using AutoMapper;
using BlogApp.Application.Features.AppUsers.Commands.Create;
using BlogApp.Application.Features.AppUsers.Queries.GetById;
using BlogApp.Application.Features.AppUsers.Queries.GetList;
using BlogApp.Application.Features.AppUsers.Queries.GetPaginatedListByDynamic;
using BlogApp.Application.Features.Categories.Queries.GetPaginatedListByDynamic;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Features.AppUsers.Profiles;

public sealed class AppUserProfile : Profile
{
    public AppUserProfile()
    {
        CreateMap<AppUser, GetByIdAppUserResponse>().ReverseMap();
        CreateMap<AppUser, GetListAppUserResponse>().ReverseMap();
        CreateMap<AppUser, CreateAppUserCommand>().ReverseMap();

        CreateMap<Paginate<AppUser>, PaginatedListResponse<GetListAppUserResponse>>().ReverseMap();

        CreateMap<AppUser, GetPaginatedListByDynamicUsersResponse>().ReverseMap();
        CreateMap<Paginate<AppUser>, PaginatedListResponse<GetPaginatedListByDynamicUsersResponse>>().ReverseMap();
    }
}
