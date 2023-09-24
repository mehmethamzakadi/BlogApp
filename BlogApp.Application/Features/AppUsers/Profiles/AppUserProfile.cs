using AutoMapper;
using BlogApp.Application.DTOs.AppUsers;
using BlogApp.Application.Features.AppUsers.Commands.Create;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Features.AppUsers.Profiles
{
    public class AppUserProfile : Profile
    {
        public AppUserProfile()
        {
            CreateMap<AppUser, GetByIdAppUserResponse>().ReverseMap();
            CreateMap<AppUser, GetListAppUserResponse>().ReverseMap();
            CreateMap<AppUser, CreateAppUserCommand>().ReverseMap();
        }
    }
}
