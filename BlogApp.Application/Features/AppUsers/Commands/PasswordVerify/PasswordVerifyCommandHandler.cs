using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.PasswordVerify;

public sealed class PasswordVerifyCommandHandler(IAuthService authService) : IRequestHandler<PasswordVerifyCommand, IDataResult<bool>>
{
    public async Task<IDataResult<bool>> Handle(PasswordVerifyCommand request, CancellationToken cancellationToken)
    {
        return await authService.PasswordVerify(request.ResetToken, request.UserId);
    }
}
