
using AutoMapper;
using BlogApp.Application.Features.Categories.Commands.Update;
using BlogApp.Application.Features.Categories.Queries.GetById;
using BlogApp.Application.Features.Categories.Queries.GetList;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Features.Categories.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, GetByIdCategoryResponse>().ReverseMap();
            CreateMap<Category, GetListCategoryResponse>().ReverseMap();
            CreateMap<Category, UpdateCategoryCommand>().ReverseMap();
        }
    }
}
