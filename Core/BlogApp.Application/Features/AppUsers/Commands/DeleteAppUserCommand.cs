using AutoMapper;
using BlogApp.Application.DTOs.Common;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Commands
{
    public class DeleteAppUserCommand : IRequest<BaseResult<DeleteAppUserCommand>>
    {
        public int Id { get; set; }

        public class DeleteUserCommandHandler : IRequestHandler<DeleteAppUserCommand, BaseResult<DeleteAppUserCommand>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public DeleteUserCommandHandler(IMapper mapper, UserManager<AppUser> userManager)
            {
                _mapper = mapper;
                _userManager = userManager;
            }

            public async Task<BaseResult<DeleteAppUserCommand>> Handle(DeleteAppUserCommand request, CancellationToken cancellationToken)
            {
                var user = _userManager.Users.Where(x => x.Id == request.Id).FirstOrDefault();
                if (user == null)
                    return BaseResult<DeleteAppUserCommand>.Failure("Kullanıcı bilgisi bulunamadı.");

                await _userManager.DeleteAsync(user);

                return BaseResult<DeleteAppUserCommand>.Success(_mapper.Map<DeleteAppUserCommand>(null));
            }
        }
    }
}
