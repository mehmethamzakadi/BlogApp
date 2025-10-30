using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.BookshelfItems.Commands.Create;

public sealed class CreateBookshelfItemCommandHandler(
    IBookshelfItemRepository bookshelfItemRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateBookshelfItemCommand, IResult>
{
    public async Task<IResult> Handle(CreateBookshelfItemCommand request, CancellationToken cancellationToken)
    {
        var item = BookshelfItem.Create(
            request.Title.Trim(),
            NormalizeOptionalText(request.Author),
            NormalizeOptionalText(request.Publisher)
        );

        item.PageCount = request.PageCount is > 0 ? request.PageCount : null;
        item.IsRead = request.IsRead;
        item.Notes = NormalizeOptionalText(request.Notes);
        item.ReadDate = NormalizeReadDate(request.ReadDate, request.IsRead);
        item.ImageUrl = NormalizeOptionalText(request.ImageUrl);

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
