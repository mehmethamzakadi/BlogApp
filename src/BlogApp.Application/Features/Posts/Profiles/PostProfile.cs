using AutoMapper;
using BlogApp.Application.Features.Posts.Commands.Create;
using BlogApp.Application.Features.Posts.Commands.Delete;
using BlogApp.Application.Features.Posts.Commands.Update;
using BlogApp.Application.Features.Posts.Queries.GetById;
using BlogApp.Application.Features.Posts.Queries.GetList;
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

        CreateMap<Post, GetListPostResponse>().ReverseMap();
        CreateMap<Post, GetByIdPostResponse>().ReverseMap();
        CreateMap<Paginate<Post>, PaginatedListResponse<GetListPostResponse>>().ReverseMap();
    }
}
