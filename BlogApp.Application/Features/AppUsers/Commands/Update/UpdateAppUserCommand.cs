using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Commands.Update
{
    public class UpdateAppUserCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public class UpdateUserCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<UpdateAppUserCommand, IResult>
        {
            public async Task<IResult> Handle(UpdateAppUserCommand request, CancellationToken cancellationToken)
            {
                AppUser? user = userManager.Users.Where(x => x.Id == request.Id).FirstOrDefault();
                if (user is null)
                    return new ErrorResult("Kullanıcı Bilgisi Bulunamadı!");

                user.Email = request.Email;
                user.UserName = request.UserName;

                var response = await userManager.UpdateAsync(user);
                if (!response.Succeeded)
                    return new ErrorResult("Güncelleme işlemi sırasında hata oluştu!");

                return new SuccessResult("Kullanıcı bilgisi başarıyla güncellendi.");
            }
        }
    }
}
