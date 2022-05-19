﻿using AutoMapper;
using BlogApp.Application.Utilities.Results;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Commands
{
    public class CreateAppUserCommand : IRequest<IResult>
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

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
                var user = _mapper.Map<AppUser>(request);
                var response = await _userManager.CreateAsync(user, request.Password);
                if (!response.Succeeded)
                {
                    return new ErrorResult("Ekleme işlemi sırasında hata oluştu!");
                }

                return new SuccessResult("Kullanıcı bilgisi başarıyla eklendi.");
            }
        }
    }
}