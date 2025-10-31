using BlogApp.Application.Abstractions.Images;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.BookshelfItems.Commands.Update;

public sealed class UpdateBookshelfItemCommandHandler(
    IBookshelfItemRepository bookshelfItemRepository,
    IUnitOfWork unitOfWork,
    IImageStorageService imageStorageService) : IRequestHandler<UpdateBookshelfItemCommand, IResult>
{
    public async Task<IResult> Handle(UpdateBookshelfItemCommand request, CancellationToken cancellationToken)
    {
        var item = await bookshelfItemRepository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (item is null)
            return new ErrorResult("Kitap kaydı bulunamadı.");

        var originalImageUrl = item.ImageUrl;
        string? newImageUrl = null;

        if (request.RemoveImage)
        {
            if (!string.IsNullOrWhiteSpace(originalImageUrl))
                await imageStorageService.DeleteAsync(originalImageUrl, cancellationToken);
        }
        else
        {
            var normalizedImageUrl = NormalizeOptionalText(request.ImageUrl);
            if (!string.IsNullOrWhiteSpace(normalizedImageUrl) &&
                !string.Equals(normalizedImageUrl, originalImageUrl, StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(originalImageUrl))
                    await imageStorageService.DeleteAsync(originalImageUrl, cancellationToken);
                newImageUrl = normalizedImageUrl;
            }
            else
            {
                newImageUrl = originalImageUrl;
            }
        }

        item.Update(
            request.Title.Trim(),
            NormalizeOptionalText(request.Author),
            NormalizeOptionalText(request.Publisher),
            request.PageCount is > 0 ? request.PageCount : null,
            NormalizeOptionalText(request.Notes),
            newImageUrl
        );

        item.IsRead = request.IsRead;
        item.ReadDate = NormalizeReadDate(request.ReadDate, request.IsRead);

        bookshelfItemRepository.Update(item);
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
