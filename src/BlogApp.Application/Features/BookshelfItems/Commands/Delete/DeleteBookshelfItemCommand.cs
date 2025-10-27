using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.BookshelfItems.Commands.Delete;

public sealed record DeleteBookshelfItemCommand(Guid Id) : IRequest<IResult>;
