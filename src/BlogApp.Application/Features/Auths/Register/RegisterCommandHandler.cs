
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.Auths.Register;

public sealed class RegisterCommandHandler(IUserService userService) : IRequestHandler<RegisterCommand, IResult>
{
    public async Task<IResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        AppUser? existingUser = await userService.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return new ErrorResult("Bu e-posta adresi zaten kullanılıyor!");
        }

        var user = new AppUser
        {
            Email = request.Email,
            UserName = request.UserName,
        };

        IdentityResult creationResult = await userService.CreateAsync(user, request.Password);
        if (!creationResult.Succeeded)
        {
            List<string> errors = creationResult.Errors.Select(error => error.Description).ToList();
            string message = "Kullanıcı oluşturulurken hatalar oluştu";

            return new ErrorResult(message, errors);
        }

        await userService.AddToRoleAsync(user, UserRoles.User);
        return new SuccessResult("Kayıt işlemi başarılı. Giriş yapabilirsiniz.");
    }
}
