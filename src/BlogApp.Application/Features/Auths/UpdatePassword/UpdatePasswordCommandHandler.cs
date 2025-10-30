using BlogApp.Domain.Common;
using BlogApp.Domain.Exceptions;
using BlogApp.Domain.Repositories;
using BlogApp.Domain.Services;
using MediatR;

namespace BlogApp.Application.Features.Auths.UpdatePassword;

public sealed class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, UpdatePasswordResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDomainService _userDomainService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePasswordCommandHandler(
        IUserRepository userRepository,
        IUserDomainService userDomainService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userDomainService = userDomainService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdatePasswordResponse> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        if (!request.Password.Equals(request.PasswordConfirm))
            throw new PasswordChangeFailedException("Girilen şifre aynı değil, lütfen şifreyi doğrulayınız!");

        var user = await _userRepository.GetAsync(u => u.Id == Guid.Parse(request.UserId));
        if (user == null)
            throw new PasswordChangeFailedException("Kullanıcı bulunamadı.");

        var result = _userDomainService.ResetPassword(user, request.ResetToken, request.Password);
        if (!result.Success)
            throw new PasswordChangeFailedException(result.Message ?? "Şifre güncellenemedi.");

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new();
    }
}
