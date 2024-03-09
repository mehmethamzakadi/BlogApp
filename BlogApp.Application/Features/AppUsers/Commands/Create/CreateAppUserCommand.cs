﻿using AutoMapper;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Commands.Create
{
    public class CreateAppUserCommand : IRequest<IResult>
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public class CreateUserCommandHandler : IRequestHandler<CreateAppUserCommand, IResult>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public CreateUserCommandHandler(IMapper mapper, UserManager<AppUser> userManager)
            {
                _mapper = mapper;
                _userManager = userManager;
            }

            public async Task<IResult> Handle(CreateAppUserCommand request, CancellationToken cancellationToken)
            {
                var userExists = await _userManager.FindByEmailAsync(request.Email);
                if (userExists != null)
                    return new ErrorResult("Böyle bir kullanıcı zaten sistemde mevcut!");

                var user = _mapper.Map<AppUser>(request);
                var response = await _userManager.CreateAsync(user, request.Password);
                if (!response.Succeeded)
                    return new ErrorResult("Ekleme işlemi sırasında hata oluştu!");

                //Oluşturulan her yeni kullanıcıya default olarak User rolü atanır.
                await _userManager.AddToRoleAsync(user, UserRoles.User);

                return new SuccessResult("Kullanıcı bilgisi başarıyla eklendi.");
            }
        }
    }
}
