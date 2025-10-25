using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Auths.Register;

public sealed class RegisterCommandHandler(IUserRepository userRepository) : IRequestHandler<RegisterCommand, IResult>
{
    public async Task<IResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        User? existingUser = await userRepository.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return new ErrorResult("Bu e-posta adresi zaten kullanılıyor!");
        }

        var user = new User
        {
            Email = request.Email,
            UserName = request.UserName,
            NormalizedUserName = request.UserName.ToUpperInvariant(),
            NormalizedEmail = request.Email.ToUpperInvariant(),
            PasswordHash = string.Empty // CreateAsync metodunda set edilecek
        };

        IResult creationResult = await userRepository.CreateAsync(user, request.Password);
        if (!creationResult.Success)
        {
            return creationResult;
        }

        await userRepository.AddToRoleAsync(user, UserRoles.User);
        return new SuccessResult("Kayıt işlemi başarılı. Giriş yapabilirsiniz.");
    }
}
