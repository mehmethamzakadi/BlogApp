using AutoMapper;
using BlogApp.Application.DTOs.AppUsers;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Commands
{
    public class UpdateAppUserCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public class UpdateUserCommandHandler : IRequestHandler<UpdateAppUserCommand, Unit>
        {
            private readonly UserManager<AppUser> _userManager;

            public UpdateUserCommandHandler( UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<Unit> Handle(UpdateAppUserCommand request, CancellationToken cancellationToken)
            {
                var user = _userManager.Users.Where(x => x.Id == request.Id).FirstOrDefault();
                user.Email = request.Email;
                user.UserName = request.UserName;

                await _userManager.UpdateAsync(user);

                return Unit.Value;
            }
        }
    }
}
