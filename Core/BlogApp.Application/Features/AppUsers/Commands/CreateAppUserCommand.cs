using AutoMapper;
using BlogApp.Application.DTOs.AppUsers;
using BlogApp.Application.DTOs.Common;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Commands
{
    public class CreateAppUserCommand : IRequest<bool>
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public class CreateUserCommandHandler : IRequestHandler<CreateAppUserCommand, bool>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public CreateUserCommandHandler(IMapper mapper, UserManager<AppUser> userManager)
            {
                _mapper = mapper;
                _userManager = userManager;
            }

            public async Task<bool> Handle(CreateAppUserCommand request, CancellationToken cancellationToken)
            {
                var user = _mapper.Map<AppUser>(request);
                var response = await _userManager.CreateAsync(user, request.Password);

                return response.Succeeded;
            }
        }
    }
}
