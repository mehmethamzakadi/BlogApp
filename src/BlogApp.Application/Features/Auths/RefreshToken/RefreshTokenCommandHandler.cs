using BlogApp.Application.Abstractions.Identity;
using BlogApp.Application.Features.Auths.Login;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Auths.RefreshToken;

public sealed class RefreshTokenCommandHandler(IAuthService authService) : IRequestHandler<RefreshTokenCommand, IDataResult<LoginResponse>>
{
    public async Task<IDataResult<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await authService.RefreshTokenAsync(request.RefreshToken);
    }
}
