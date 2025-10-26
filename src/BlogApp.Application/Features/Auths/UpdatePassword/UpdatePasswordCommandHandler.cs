using BlogApp.Domain.Common;
using BlogApp.Domain.Exceptions;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Auths.UpdatePassword;

public sealed class UpdatePasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdatePasswordCommand, UpdatePasswordResponse>
{
    public async Task<UpdatePasswordResponse> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        if (!request.Password.Equals(request.PasswordConfirm))
            throw new PasswordChangeFailedException("Girilen şifre aynı değil, lütfen şifreyi doğrulayınız!");

        var result = await userRepository.UpdatePasswordAsync(Guid.Parse(request.UserId), request.ResetToken, request.Password);
        if (!result.Success)
            throw new PasswordChangeFailedException(result.Message ?? "Şifre güncellenemedi.");

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new();
    }
}
