using AutoMapper;
using BlogApp.Application.DTOs;
using BlogApp.Application.DTOs.Common;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Commands
{
    public class UpdateAppUserCommand : IRequest<BaseResult<AppUserUpdateDto>>
    {
        public AppUserUpdateDto User { get; set; }

        public class UpdateUserCommandHandler : IRequestHandler<UpdateAppUserCommand, BaseResult<AppUserUpdateDto>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public UpdateUserCommandHandler(IMapper mapper, UserManager<AppUser> userManager)
            {
                _mapper = mapper;
                _userManager = userManager;
            }

            public async Task<BaseResult<AppUserUpdateDto>> Handle(UpdateAppUserCommand request, CancellationToken cancellationToken)
            {
                var user = _userManager.Users.Where(x => x.Id == request.User.Id).FirstOrDefault();
                if (user == null)
                    return BaseResult<AppUserUpdateDto>.Failure("Kullanıcı bilgisi bulunamadı.");

                user.Email = request.User.Email;
                user.UserName = request.User.UserName;

                await _userManager.UpdateAsync(user);

                return BaseResult<AppUserUpdateDto>.Success(_mapper.Map<AppUserUpdateDto>(user));
            }
        }
    }
}
