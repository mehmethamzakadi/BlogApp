using AutoMapper;
using BlogApp.Application.DTOs.Params;
using BlogApp.Application.DTOs.Results;
using BlogApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Application.Profiles
{
    public class AppUserProfile : Profile
    {
        public AppUserProfile()
        {
            CreateMap<AppUser, RsAppUser>().ReverseMap();
            CreateMap<AppUser, PmAppUser>().ReverseMap();
        }
    }
}
