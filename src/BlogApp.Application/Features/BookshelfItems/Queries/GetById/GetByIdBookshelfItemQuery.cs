using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.BookshelfItems.Queries.GetById;

public sealed record GetByIdBookshelfItemQuery(Guid Id) : IRequest<IDataResult<GetByIdBookshelfItemResponse>>;
