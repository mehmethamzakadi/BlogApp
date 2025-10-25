using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.UserEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.AppUsers.Commands.Delete;

public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteAppUserCommand, IResult>
{
    private readonly IUserService _userManager;
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(
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

    public async Task<IResult> Handle(DeleteAppUserCommand request, CancellationToken cancellationToken)
    {
        var user = _userManager.FindById(request.Id);
        if (user == null)
            return new ErrorResult("Kullanıcı bilgisi bulunamadı!");

        // Event için bilgileri sakla (silindikten sonra erişemeyebiliriz)
        var userId = user.Id;
        var userName = user.UserName ?? "";
        var userEmail = user.Email ?? "";

        var response = await _userManager.DeleteAsync(user);
        if (!response.Succeeded)
            return new ErrorResult("Silme işlemi sırasında hata oluştu!");

        // ✅ AppUser artık AddDomainEvent() metoduna sahip
        var currentUserId = _currentUserService.GetCurrentUserId();
        user.AddDomainEvent(new UserDeletedEvent(userId, userName, userEmail, currentUserId));

        // UnitOfWork SaveChanges sırasında domain event'leri otomatik olarak Outbox'a kaydeder
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Kullanıcı bilgisi başarıyla silindi.");
    }
}
