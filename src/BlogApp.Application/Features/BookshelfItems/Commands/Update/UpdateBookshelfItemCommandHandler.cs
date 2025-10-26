using System;
using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.BookshelfItemEvents;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.BookshelfItems.Commands.Update;

public sealed class UpdateBookshelfItemCommandHandler(
    IBookshelfItemRepository bookshelfItemRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateBookshelfItemCommand, IResult>
{
    public async Task<IResult> Handle(UpdateBookshelfItemCommand request, CancellationToken cancellationToken)
    {
        BookshelfItem? item = await bookshelfItemRepository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (item is null)
        {
            return new ErrorResult("Kitap kaydı bulunamadı.");
        }

        item.Title = request.Title.Trim();
        item.Author = NormalizeOptionalText(request.Author);
        item.Publisher = NormalizeOptionalText(request.Publisher);
        item.PageCount = request.PageCount is > 0 ? request.PageCount : null;
        item.IsRead = request.IsRead;
        item.Notes = NormalizeOptionalText(request.Notes);
        item.ReadDate = NormalizeReadDate(request.ReadDate, request.IsRead);

        var actorId = currentUserService.GetCurrentUserId() ?? SystemUsers.SystemUserId;
        item.AddDomainEvent(new BookshelfItemUpdatedEvent(item.Id, item.Title, actorId));

        await bookshelfItemRepository.UpdateAsync(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Kitap kaydı başarıyla güncellendi.");
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
