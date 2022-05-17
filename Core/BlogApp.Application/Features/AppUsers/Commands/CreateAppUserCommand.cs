using AutoMapper;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;
using BlogApp.Domain.Entities;
using BlogApp.Application.DTOs.Common;
using Microsoft.AspNetCore.Identity;
using BlogApp.Application.DTOs;

namespace BlogApp.Application.Features.AppUsers.Commands
{
    public class CreateAppUserCommand : IRequest<BaseResult<CreateAppUserCommand>>
    {
        public AppUserCreateDto User { get; set; }

        public class CreateUserCommandHandler : IRequestHandler<CreateAppUserCommand, BaseResult<CreateAppUserCommand>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public CreateUserCommandHandler(IMapper mapper, UserManager<AppUser> userManager)
            {
                _mapper = mapper;
                _userManager = userManager;
            }

            public async Task<BaseResult<CreateAppUserCommand>> Handle(CreateAppUserCommand request, CancellationToken cancellationToken)
            {
                var user = _mapper.Map<AppUser>(request.User);
                await _userManager.CreateAsync(user, request.User.Password);
                return BaseResult<CreateAppUserCommand>.Success(null);

            }
        }
    }
}
