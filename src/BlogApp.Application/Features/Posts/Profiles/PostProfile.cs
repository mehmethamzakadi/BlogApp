using AutoMapper;
using BlogApp.Application.Features.Posts.Commands.Create;
using BlogApp.Application.Features.Posts.Commands.Delete;
using BlogApp.Application.Features.Posts.Commands.Update;
using BlogApp.Application.Features.Posts.Queries.GetById;
using BlogApp.Application.Features.Posts.Queries.GetList;
using BlogApp.Application.Features.Posts.Queries.GetListByCategoryId;
using BlogApp.Application.Features.Posts.Queries.GetPaginatedListByDynamic;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Features.Posts.Profiles;

public sealed class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<Post, CreatePostCommand>().ReverseMap();
        CreateMap<Post, UpdatePostCommand>().ReverseMap();
        CreateMap<Post, DeletePostCommand>().ReverseMap();

        CreateMap<Post, GetPaginatedListByDynamicPostsResponse>().ReverseMap();

        CreateMap<Post, GetListPostResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));

        CreateMap<Post, GetListPostByCategoryIdResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));

        CreateMap<Post, GetByIdPostResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ReverseMap();

        CreateMap<Paginate<Post>, PaginatedListResponse<GetPaginatedListByDynamicPostsResponse>>().ReverseMap();
        CreateMap<Paginate<Post>, PaginatedListResponse<GetListPostResponse>>().ReverseMap();
        CreateMap<Paginate<Post>, PaginatedListResponse<GetListPostByCategoryIdResponse>>().ReverseMap();
    }
}
