using AutoMapper;
using BlogApp.Application.DTOs.ParamsDTOs.Category;
using BlogApp.Application.DTOs.ResultDTOs.Category;
using BlogApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Application.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, RsCategoryDto>().ReverseMap();
            CreateMap<Category, PmCategoryDto>().ReverseMap();
        }
    }
}
