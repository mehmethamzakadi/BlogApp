using System;
using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.BookshelfItemEvents;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.BookshelfItems.Commands.Create;

public sealed class CreateBookshelfItemCommandHandler(
    IBookshelfItemRepository bookshelfItemRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<CreateBookshelfItemCommand, IResult>
{
    public async Task<IResult> Handle(CreateBookshelfItemCommand request, CancellationToken cancellationToken)
    {
        var item = new BookshelfItem
        {
            Title = request.Title.Trim(),
            Author = NormalizeOptionalText(request.Author),
            Publisher = NormalizeOptionalText(request.Publisher),
            PageCount = request.PageCount is > 0 ? request.PageCount : null,
            IsRead = request.IsRead,
            Notes = NormalizeOptionalText(request.Notes),
            ReadDate = NormalizeReadDate(request.ReadDate, request.IsRead)
        };

        var actorId = currentUserService.GetCurrentUserId() ?? SystemUsers.SystemUserId;
        item.AddDomainEvent(new BookshelfItemCreatedEvent(item.Id, item.Title, actorId));

        await bookshelfItemRepository.AddAsync(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Kitap kaydı başarıyla eklendi.");
    }

    private static string? NormalizeOptionalText(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static DateTime? NormalizeReadDate(DateTime? readDate, bool isRead)
    {
        if (!isRead || readDate is null)
        {
            return null;
        }

        var dateOnly = readDate.Value.Date;
        return DateTime.SpecifyKind(dateOnly, DateTimeKind.Utc);
    }
}
