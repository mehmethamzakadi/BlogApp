using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Auths.PasswordReset;

public sealed class PasswordResetCommandHandler(IAuthService authService) : IRequestHandler<PasswordResetCommand, IResult>
{

    public async Task<IResult> Handle(PasswordResetCommand request, CancellationToken cancellationToken)
    {
        await authService.PasswordResetAsync(request.Email);
        return new SuccessResult("Şifre yenileme işlemleri için mail gönderildi.");
    }
}
