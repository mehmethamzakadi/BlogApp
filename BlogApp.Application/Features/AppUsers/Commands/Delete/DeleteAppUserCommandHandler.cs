using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Commands.Delete;

public sealed class DeleteUserCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<DeleteAppUserCommand, IResult>
{
    public async Task<IResult> Handle(DeleteAppUserCommand request, CancellationToken cancellationToken)
    {
        var user = userManager.Users.Where(x => x.Id == request.Id).FirstOrDefault();
        if (user == null)
            return new ErrorResult("Kullanıcı bilgisi bulunamadı!");

        var response = await userManager.DeleteAsync(user);
        if (!response.Succeeded)
            return new ErrorResult("Silme işlemi sırasında hata oluştu!");

        return new SuccessResult("Kullanıcı bilgisi başarıyla silindi.");
    }
}
