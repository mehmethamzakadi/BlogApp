using AutoMapper;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Commands
{
    public class DeleteAppUserCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public class DeleteUserCommandHandler : IRequestHandler<DeleteAppUserCommand, Unit>
        {
            private readonly UserManager<AppUser> _userManager;

            public DeleteUserCommandHandler( UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<Unit> Handle(DeleteAppUserCommand request, CancellationToken cancellationToken)
            {
                var user = _userManager.Users.Where(x => x.Id == request.Id).FirstOrDefault();
                await _userManager.DeleteAsync(user);

                return Unit.Value;
            }
        }
    }
}
