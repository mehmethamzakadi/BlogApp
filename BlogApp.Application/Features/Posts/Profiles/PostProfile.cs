using AutoMapper;
using BlogApp.Application.Features.Posts.Commands.Update;
using BlogApp.Application.Features.Posts.Queries.GetById;
using BlogApp.Application.Features.Posts.Queries.GetList;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Features.Posts.Profiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, GetByIdPostResponse>().ReverseMap();
            CreateMap<Post, GetListPostResponse>().ReverseMap();
            CreateMap<Post, UpdatePostCommand>().ReverseMap();
        }
    }
}
