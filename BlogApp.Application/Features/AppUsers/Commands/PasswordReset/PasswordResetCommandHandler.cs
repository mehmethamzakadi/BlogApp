﻿using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.PasswordReset;

public sealed class PasswordResetCommandHandler(IAuthService authService) : IRequestHandler<PasswordResetCommand, IResult>
{

    public async Task<IResult> Handle(PasswordResetCommand request, CancellationToken cancellationToken)
    {
        await authService.PasswordResetAsync(request.Email);
        return new SuccessResult("Şifre yenileme işlemleri için mail gönderildi.");
    }
}
