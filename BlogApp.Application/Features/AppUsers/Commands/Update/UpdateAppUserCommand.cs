﻿using BlogApp.Application.Utilities.Results;
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

        public class UpdateUserCommandHandler : IRequestHandler<UpdateAppUserCommand, IResult>
        {
            private readonly UserManager<AppUser> _userManager;

            public UpdateUserCommandHandler(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<IResult> Handle(UpdateAppUserCommand request, CancellationToken cancellationToken)
            {
                AppUser? user = _userManager.Users.Where(x => x.Id == request.Id).FirstOrDefault();
                if (user is null)
                    return new ErrorResult("Kullanıcı Bilgisi Bulunamadı!");

                user.Email = request.Email;
                user.UserName = request.UserName;

                var response = await _userManager.UpdateAsync(user);
                if (!response.Succeeded)
                    return new ErrorResult("Güncelleme işlemi sırasında hata oluştu!");

                return new SuccessResult("Kullanıcı bilgisi başarıyla güncellendi.");
            }
        }
    }
}
