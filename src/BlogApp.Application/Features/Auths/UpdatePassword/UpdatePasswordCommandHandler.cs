using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Exceptions;
using MediatR;

namespace BlogApp.Application.Features.Auths.UpdatePassword;

public sealed class UpdatePasswordCommandHandler(IUserService userService) : IRequestHandler<UpdatePasswordCommand, UpdatePasswordResponse>
{
    public async Task<UpdatePasswordResponse> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        if (!request.Password.Equals(request.PasswordConfirm))
            throw new PasswordChangeFailedException("Girilen şifre aynı değil, lütfen şifreyi doğrulayınız!");

        await userService.UpdatePasswordAsync(request.UserId, request.ResetToken, request.Password);
        return new();
    }
}
