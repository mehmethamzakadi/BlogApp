using AutoMapper;
using BlogApp.Application.DTOs;
using BlogApp.Application.Features.AppUsers.Commands;
using BlogApp.Application.Features.Categories.Commands;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Profiles
{
    public class AppUserProfile : Profile
    {
        public AppUserProfile()
        {
            CreateMap<AppUser, AppUserDto>().ReverseMap();
            CreateMap<AppUser, AppUserCreateDto>().ReverseMap();
            CreateMap<AppUser, AppUserUpdateDto>().ReverseMap();
            CreateMap<AppUser, UpdateAppUserCommand>().ReverseMap();
            CreateMap<AppUser, DeleteAppUserCommand>().ReverseMap();
        }
    }
}
