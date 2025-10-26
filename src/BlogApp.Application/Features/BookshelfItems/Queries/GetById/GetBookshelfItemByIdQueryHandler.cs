using AutoMapper;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.BookshelfItems.Queries.GetById;

public sealed class GetBookshelfItemByIdQueryHandler(
    IBookshelfItemRepository bookshelfItemRepository,
    IMapper mapper) : IRequestHandler<GetByIdBookshelfItemQuery, IDataResult<GetByIdBookshelfItemResponse>>
{
    public async Task<IDataResult<GetByIdBookshelfItemResponse>> Handle(GetByIdBookshelfItemQuery request, CancellationToken cancellationToken)
    {
        BookshelfItem? item = await bookshelfItemRepository.GetAsync(
            predicate: x => x.Id == request.Id,
            cancellationToken: cancellationToken);

        if (item is null)
        {
            return new ErrorDataResult<GetByIdBookshelfItemResponse>("Kitap kaydı bulunamadı.");
        }

        GetByIdBookshelfItemResponse response = mapper.Map<GetByIdBookshelfItemResponse>(item);

        return new SuccessDataResult<GetByIdBookshelfItemResponse>(response);
    }
}
