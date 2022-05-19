using AutoMapper;
using BlogApp.Application.DTOs.Categories;
using BlogApp.Application.Features.Categories.Commands;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryResponseDto>().ReverseMap();
        }
    }
}
