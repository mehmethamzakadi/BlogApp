using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.BookshelfItems.Commands.Create;

public sealed record CreateBookshelfItemCommand(
    string Title,
    string? Author,
    string? Publisher,
    int? PageCount,
    bool IsRead,
    string? Notes,
    DateTime? ReadDate,
    string? ImageUrl
) : IRequest<IResult>;
