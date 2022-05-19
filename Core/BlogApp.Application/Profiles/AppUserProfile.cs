using AutoMapper;
using BlogApp.Application.DTOs;
using BlogApp.Application.DTOs.AppUsers;
using BlogApp.Application.Features.AppUsers.Commands;
using BlogApp.Application.Features.Categories.Commands;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Profiles
{
    public class AppUserProfile : Profile
    {
        public AppUserProfile()
        {
            CreateMap<AppUser, AppUserResponseDto>().ReverseMap();
        }
    }
}
