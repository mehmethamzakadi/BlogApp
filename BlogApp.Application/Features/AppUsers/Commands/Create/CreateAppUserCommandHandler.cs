using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.Create;

public sealed class CreateUserCommandHandler(IUserService userService) : IRequestHandler<CreateAppUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateAppUserCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = await userService.FindByEmailAsync(request.Email);
        if (user != null)
            return Result<string>.FailureResult("Böyle bir kullanıcı zaten sistemde mevcut!");

        string message = "Kullanıcı bilgisi başarıyla eklendi.";

        user = new AppUser { Email = request.Email, UserName = request.UserName };
        var result = await userService.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            //Oluşturulan her yeni kullanıcıya default olarak User rolü atanır.
            await userService.AddToRoleAsync(user!, UserRoles.User);
            return Result<string>.SuccessResult(message);
        }
        else
        {
            foreach (var error in result.Errors)
            {
                message += $"{error.Code}-{error.Description}";
            }
            return Result<string>.FailureResult(message);
        }
    }
}
