using BlogApp.Application.Utilities.Results;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Commands
{
    public class DeleteAppUserCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteUserCommandHandler : IRequestHandler<DeleteAppUserCommand, IResult>
        {
            private readonly UserManager<AppUser> _userManager;

            public DeleteUserCommandHandler(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<IResult> Handle(DeleteAppUserCommand request, CancellationToken cancellationToken)
            {
                var user = _userManager.Users.Where(x => x.Id == request.Id).FirstOrDefault();
                if (user == null)
                    return new ErrorResult("Kullanıcı bilgisi bulunamadı!");

                var response = await _userManager.DeleteAsync(user);
                if (!response.Succeeded)
                    return new ErrorResult("Silme işlemi sırasında hata oluştu!");

                return new SuccessResult("Kullanıcı bilgisi başarıyla silindi.");
            }
        }
    }
}
