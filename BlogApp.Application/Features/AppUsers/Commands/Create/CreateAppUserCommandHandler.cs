using AutoMapper;
using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.Create;

public sealed class CreateUserCommandHandler(IMapper mapper, IUserService userService) : IRequestHandler<CreateAppUserCommand, IResult>
{
    public async Task<IResult> Handle(CreateAppUserCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = await userService.FindByEmailAsync(request.Email);
        if (user != null)
            return new ErrorResult("Böyle bir kullanıcı zaten sistemde mevcut!");

        string message = "Kullanıcı bilgisi başarıyla eklendi.";
        var result = await userService.CreateAsync(user!, request.Password);
        if (result.Succeeded)
        {
            //Oluşturulan her yeni kullanıcıya default olarak User rolü atanır.
            await userService.AddToRoleAsync(user!, UserRoles.User);
            return new SuccessResult(message);
        }
        else
        {
            foreach (var error in result.Errors)
            {
                message += $"{error.Code}-{error.Description}";
            }
            return new ErrorResult(message);
        }
    }
}
