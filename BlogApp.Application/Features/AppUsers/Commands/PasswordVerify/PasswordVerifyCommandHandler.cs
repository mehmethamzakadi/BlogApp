using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.PasswordVerify;

public sealed class PasswordVerifyCommandHandler(IAuthService authService) : IRequestHandler<PasswordVerifyCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(PasswordVerifyCommand request, CancellationToken cancellationToken)
    {
        return await authService.PasswordVerify(request.ResetToken, request.UserId);
    }
}
