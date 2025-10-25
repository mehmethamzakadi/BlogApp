using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Events.UserEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Users.Commands.Delete;

public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        IMediator mediator,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _mediator = mediator;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = _userRepository.FindById(request.Id);
        if (user == null)
            return new ErrorResult("Kullanıcı bilgisi bulunamadı!");

        // ✅ Silme işleminden ÖNCE domain event'i tetikle
        var userId = user.Id;
        var userName = user.UserName ?? "";
        var userEmail = user.Email ?? "";
        var currentUserId = _currentUserService.GetCurrentUserId();
        user.AddDomainEvent(new UserDeletedEvent(userId, userName, userEmail, currentUserId));

        var response = await _userRepository.DeleteUserAsync(user);
        if (!response.Success)
            return response;

        // UnitOfWork SaveChanges sırasında domain event'leri otomatik olarak Outbox'a kaydeder
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Kullanıcı bilgisi başarıyla silindi.");
    }
}
