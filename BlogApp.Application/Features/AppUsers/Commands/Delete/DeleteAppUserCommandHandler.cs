using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.Delete;

public sealed class DeleteUserCommandHandler(IUserService userManager) : IRequestHandler<DeleteAppUserCommand, IResult>
{
    public async Task<IResult> Handle(DeleteAppUserCommand request, CancellationToken cancellationToken)
    {
        var user = userManager.FindById(request.Id);
        if (user == null)
            return new ErrorResult("Kullanıcı bilgisi bulunamadı!");

        var response = await userManager.DeleteAsync(user);
        if (!response.Succeeded)
            return new ErrorResult("Silme işlemi sırasında hata oluştu!");

        return new SuccessResult("Kullanıcı bilgisi başarıyla silindi.");
    }
}
