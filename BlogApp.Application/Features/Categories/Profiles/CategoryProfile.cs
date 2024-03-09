
using AutoMapper;
using BlogApp.Application.Features.Categories.Commands.Create;
using BlogApp.Application.Features.Categories.Commands.Delete;
using BlogApp.Application.Features.Categories.Commands.Update;
using BlogApp.Application.Features.Categories.Queries.GetById;
using BlogApp.Application.Features.Categories.Queries.GetList;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Features.Categories.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CreateCategoryCommand>().ReverseMap();
            CreateMap<Category, UpdateCategoryCommand>().ReverseMap();
            CreateMap<Category, DeleteCategoryCommand>().ReverseMap();

            CreateMap<Category, GetListCategoryResponse>().ReverseMap();
            CreateMap<Category, GetByIdCategoryResponse>().ReverseMap();
            CreateMap<Paginate<Category>, GetListResponse<GetListCategoryResponse>>().ReverseMap();

        }
    }
}
