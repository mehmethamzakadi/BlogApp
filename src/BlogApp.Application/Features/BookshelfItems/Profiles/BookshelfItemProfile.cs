using AutoMapper;
using BlogApp.Application.Features.BookshelfItems.Commands.Create;
using BlogApp.Application.Features.BookshelfItems.Commands.Delete;
using BlogApp.Application.Features.BookshelfItems.Commands.Update;
using BlogApp.Application.Features.BookshelfItems.Queries.GetById;
using BlogApp.Application.Features.BookshelfItems.Queries.GetPaginatedListByDynamic;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Features.BookshelfItems.Profiles;

public sealed class BookshelfItemProfile : Profile
{
    public BookshelfItemProfile()
    {
        CreateMap<BookshelfItem, CreateBookshelfItemCommand>().ReverseMap();
        CreateMap<BookshelfItem, UpdateBookshelfItemCommand>().ReverseMap();
        CreateMap<BookshelfItem, DeleteBookshelfItemCommand>().ReverseMap();

        CreateMap<BookshelfItem, GetPaginatedListByDynamicBookshelfItemsResponse>().ReverseMap();
        CreateMap<BookshelfItem, GetByIdBookshelfItemResponse>().ReverseMap();

        CreateMap<Paginate<BookshelfItem>, PaginatedListResponse<GetPaginatedListByDynamicBookshelfItemsResponse>>().ReverseMap();
    }
}
