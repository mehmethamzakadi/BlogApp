using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.PasswordReset;

public sealed class PasswordResetCommandHandler(IAuthService authService) : IRequestHandler<PasswordResetCommand, Result<string>>
{

    public async Task<Result<string>> Handle(PasswordResetCommand request, CancellationToken cancellationToken)
    {
        await authService.PasswordResetAsync(request.Email);
        return Result<string>.SuccessResult("Şifre yenileme işlemleri için mail gönderildi.");
    }
}
