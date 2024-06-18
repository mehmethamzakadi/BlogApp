using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.UpdatePassword;

public sealed class UpdatePasswordCommandHandler(IUserService userService)
    : IRequestHandler<UpdatePasswordCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        if (!request.Password.Equals(request.PasswordConfirm))
            return Result<string>.FailureResult("Girilen şifre aynı değil, lütfen şifreyi doğrulayınız!");

        await userService.UpdatePasswordAsync(request.UserId, request.ResetToken, request.Password);

        return Result<string>.SuccessResult("Güncelleme işlemi başarılı");
    }
}
