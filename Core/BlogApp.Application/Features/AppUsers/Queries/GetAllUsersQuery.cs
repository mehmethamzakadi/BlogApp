using AutoMapper;
using BlogApp.Application.DTOs.AppUsers;
using BlogApp.Application.DTOs.Common;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Queries
{
    public class GetAllUsersQuery : IRequest<IReadOnlyList<AppUserResponseDto>>
    {

        public class GetAllUserQueryHandler : IRequestHandler<GetAllUsersQuery, IReadOnlyList<AppUserResponseDto>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public GetAllUserQueryHandler(UserManager<AppUser> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<IReadOnlyList<AppUserResponseDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
            {
                var response = _mapper.Map<IReadOnlyList<AppUserResponseDto>>(_userManager.Users.ToList());
                return response;
            }
        }
    }
}
