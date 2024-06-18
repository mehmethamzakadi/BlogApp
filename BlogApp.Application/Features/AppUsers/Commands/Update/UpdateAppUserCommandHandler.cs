using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.Update;

public sealed class UpdateUserCommandHandler(IUserService userManager) : IRequestHandler<UpdateAppUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateAppUserCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = userManager.FindById(request.Id);
        if (user is null)
            return Result<string>.FailureResult("Kullanıcı Bilgisi Bulunamadı!");

        user.Email = request.Email;
        user.UserName = request.UserName;

        var response = await userManager.UpdateAsync(user);
        if (!response.Succeeded)
            return Result<string>.FailureResult("Güncelleme işlemi sırasında hata oluştu!");

        return Result<string>.SuccessResult("Kullanıcı bilgisi başarıyla güncellendi.");
    }
}
