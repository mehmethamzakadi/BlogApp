using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.Delete;

public sealed class DeleteUserCommandHandler(IUserService userManager) : IRequestHandler<DeleteAppUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DeleteAppUserCommand request, CancellationToken cancellationToken)
    {
        var user = userManager.FindById(request.Id);
        if (user == null)
            return Result<string>.FailureResult("Kullanıcı bilgisi bulunamadı!");

        var response = await userManager.DeleteAsync(user);
        if (!response.Succeeded)
            return Result<string>.FailureResult("Silme işlemi sırasında hata oluştu!");

        return Result<string>.SuccessResult("Kullanıcı bilgisi başarıyla silindi.");
    }
}
