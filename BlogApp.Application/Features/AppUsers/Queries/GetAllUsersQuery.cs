using AutoMapper;
using BlogApp.Application.DTOs.AppUsers;
using BlogApp.Application.Utilities.Results;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Queries
{
    public class GetAllUsersQuery : IRequest<IDataResult<IReadOnlyList<AppUserResponseDto>>>
    {

        public class GetAllUserQueryHandler : IRequestHandler<GetAllUsersQuery, IDataResult<IReadOnlyList<AppUserResponseDto>>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public GetAllUserQueryHandler(UserManager<AppUser> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<IDataResult<IReadOnlyList<AppUserResponseDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
            {
                var userList = _userManager.Users;
                var userListDto = userList.Select(user => _mapper.Map<AppUserResponseDto>(user)).ToList();
                return new SuccessDataResult<IReadOnlyList<AppUserResponseDto>>(userListDto);
            }
        }
    }
}
