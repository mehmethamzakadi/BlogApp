using System;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.BookshelfItems.Commands.Update;

public sealed record UpdateBookshelfItemCommand(
    Guid Id,
    string Title,
    string? Author,
    string? Publisher,
    int? PageCount,
    bool IsRead,
    string? Notes,
    DateTime? ReadDate
) : IRequest<IResult>;
