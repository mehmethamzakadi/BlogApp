using AutoMapper;
using BlogApp.Application.Abstractions;
using BlogApp.Application.Behaviors.Transaction;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.DTOs;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Commands.Create;

public class CreateAppUserCommand : IRequest<IResult>, ITransactionalRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public class CreateUserCommandHandler(IMapper mapper, UserManager<AppUser> userManager, IEmailService mailService) : IRequestHandler<CreateAppUserCommand, IResult>
    {
        public async Task<IResult> Handle(CreateAppUserCommand request, CancellationToken cancellationToken)
        {
            var userExists = await userManager.FindByEmailAsync(request.Email);
            if (userExists != null)
                return new ErrorResult("Böyle bir kullanıcı zaten sistemde mevcut!");

            var user = mapper.Map<AppUser>(request);
            var response = await userManager.CreateAsync(user, request.Password);
            if (!response.Succeeded)
                return new ErrorResult("Ekleme işlemi sırasında hata oluştu!");

            //Oluşturulan her yeni kullanıcıya default olarak User rolü atanır.
            await userManager.AddToRoleAsync(user, UserRoles.User);

            await mailService.SendEmailAsync(new EMailRequest { ToEmail = "mehmet@localhost", Subject = "Deneme Mail", Body = "Deneme içerik.." });

            return new SuccessResult("Kullanıcı bilgisi başarıyla eklendi.");
        }
    }
}
