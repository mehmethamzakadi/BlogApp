using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.UserEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Users.Commands.Update;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
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

    public async Task<IResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        User? user = _userRepository.FindById(request.Id);
        if (user is null)
            return new ErrorResult("Kullanıcı Bilgisi Bulunamadı!");

        // Email değiştiriliyorsa duplicate kontrolü (mevcut kullanıcı hariç)
        if (user.Email != request.Email)
        {
            var existingEmail = await _userRepository.FindByEmailAsync(request.Email);
            if (existingEmail != null && existingEmail.Id != request.Id)
                return new ErrorResult("Bu e-posta adresi zaten kullanılıyor!");
        }

        // Username değiştiriliyorsa duplicate kontrolü (mevcut kullanıcı hariç)
        if (user.UserName != request.UserName)
        {
            var existingUserName = await _userRepository.FindByUserNameAsync(request.UserName);
            if (existingUserName != null && existingUserName.Id != request.Id)
                return new ErrorResult("Bu kullanıcı adı zaten kullanılıyor!");
        }

        user.Email = request.Email;
        user.UserName = request.UserName;

        var response = await _userRepository.UpdateAsync(user);
        if (!response.Success)
            return response;

        // ✅ User artık AddDomainEvent() metoduna sahip (BaseEntity üzerinden)
        var currentUserId = _currentUserService.GetCurrentUserId();
        user.AddDomainEvent(new UserUpdatedEvent(user.Id, user.UserName!, user.Email!, currentUserId));

        // UnitOfWork SaveChanges sırasında domain event'leri otomatik olarak Outbox'a kaydeder
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Kullanıcı bilgisi başarıyla güncellendi.");
    }
}
