
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Commands.Create;

public sealed class CreateAppUserCommandHandler(IUserService userService) : IRequestHandler<CreateAppUserCommand, IResult>
{
    public async Task<IResult> Handle(CreateAppUserCommand request, CancellationToken cancellationToken)
    {
        AppUser? existingUser = await userService.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return new ErrorResult("Böyle bir kullanıcı zaten sistemde mevcut!");
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
        return new SuccessResult("Kullanıcı bilgisi başarıyla eklendi.");
    }
}
