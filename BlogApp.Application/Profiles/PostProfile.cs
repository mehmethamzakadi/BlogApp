using AutoMapper;
using BlogApp.Application.DTOs.Posts;
using BlogApp.Application.Features.Posts.Commands;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Profiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostResponseDto>().ReverseMap();
            CreateMap<Post, UpdatePostCommand>().ReverseMap();

        }
    }
}
