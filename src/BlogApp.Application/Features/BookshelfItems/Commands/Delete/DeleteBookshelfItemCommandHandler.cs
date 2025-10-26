using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.BookshelfItemEvents;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.BookshelfItems.Commands.Delete;

public sealed class DeleteBookshelfItemCommandHandler(
    IBookshelfItemRepository bookshelfItemRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<DeleteBookshelfItemCommand, IResult>
{
    public async Task<IResult> Handle(DeleteBookshelfItemCommand request, CancellationToken cancellationToken)
    {
        BookshelfItem? item = await bookshelfItemRepository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (item is null)
        {
            return new ErrorResult("Kitap kaydı bulunamadı.");
        }

        var actorId = currentUserService.GetCurrentUserId() ?? SystemUsers.SystemUserId;
        item.AddDomainEvent(new BookshelfItemDeletedEvent(item.Id, item.Title, actorId));

        await bookshelfItemRepository.DeleteAsync(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Kitap kaydı başarıyla silindi.");
    }
}
