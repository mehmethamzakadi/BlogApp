using AutoMapper;
using BlogApp.Application.DTOs.Params;
using BlogApp.Application.DTOs.Results;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, RsCategory>().ReverseMap();
            CreateMap<Category, PmCategory>().ReverseMap();

        }
    }
}
