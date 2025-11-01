using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.BookshelfItems.Commands.Delete;

public sealed class DeleteBookshelfItemCommandHandler(
    IBookshelfItemRepository bookshelfItemRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteBookshelfItemCommand, IResult>
{
    public async Task<IResult> Handle(DeleteBookshelfItemCommand request, CancellationToken cancellationToken)
    {
        var item = await bookshelfItemRepository.GetAsync(x => x.Id == request.Id, enableTracking: true, cancellationToken: cancellationToken);
        if (item is null)
            return new ErrorResult("Kitap kaydı bulunamadı.");

        item.Delete();
        bookshelfItemRepository.Delete(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Kitap kaydı başarıyla silindi.");
    }
}
