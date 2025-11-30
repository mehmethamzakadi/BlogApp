
using AutoMapper;
using BlogApp.Application.Features.Categories.Commands.Create;
using BlogApp.Application.Features.Categories.Commands.Delete;
using BlogApp.Application.Features.Categories.Commands.Update;
using BlogApp.Application.Features.Categories.Queries.GetAll;
using BlogApp.Application.Features.Categories.Queries.GetById;
using BlogApp.Application.Features.Categories.Queries.GetPaginatedListByDynamic;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Features.Categories.Profiles
{
    public sealed class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CreateCategoryCommand>().ReverseMap();
            CreateMap<Category, UpdateCategoryCommand>().ReverseMap();
            CreateMap<Category, DeleteCategoryCommand>().ReverseMap();

            CreateMap<Category, GetPaginatedListByDynamicCategoriesResponse>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null))
                .ReverseMap();
            CreateMap<Category, GetAllListCategoriesResponse>().ReverseMap();

            CreateMap<Category, GetByIdCategoryResponse>().ReverseMap();
            CreateMap<Paginate<Category>, PaginatedListResponse<GetPaginatedListByDynamicCategoriesResponse>>().ReverseMap();


        }
    }
}
