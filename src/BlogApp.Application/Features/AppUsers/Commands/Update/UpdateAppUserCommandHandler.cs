using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.UserEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.AppUsers.Commands.Update;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateAppUserCommand, IResult>
{
    private readonly IUserService _userManager;
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IUserService userManager,
        IMediator mediator,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _mediator = mediator;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(UpdateAppUserCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = _userManager.FindById(request.Id);
        if (user is null)
            return new ErrorResult("Kullanıcı Bilgisi Bulunamadı!");

        user.Email = request.Email;
        user.UserName = request.UserName;

        var response = await _userManager.UpdateAsync(user);
        if (!response.Succeeded)
            return new ErrorResult("Güncelleme işlemi sırasında hata oluştu!");

        // ✅ AppUser artık AddDomainEvent() metoduna sahip
        var currentUserId = _currentUserService.GetCurrentUserId();
        user.AddDomainEvent(new UserUpdatedEvent(user.Id, user.UserName!, user.Email!, currentUserId));

        // UnitOfWork SaveChanges sırasında domain event'leri otomatik olarak Outbox'a kaydeder
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Kullanıcı bilgisi başarıyla güncellendi.");
    }
}
