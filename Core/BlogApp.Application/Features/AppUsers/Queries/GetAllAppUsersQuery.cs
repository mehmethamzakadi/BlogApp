using AutoMapper;
using BlogApp.Application.DTOs.Common;
using BlogApp.Application.DTOs.Results;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Application.Features.AppUsers.Queries
{
    public class GetAllAppUsersQuery : IRequest<BaseResult<IReadOnlyList<RsAppUser>>>
    {
        public class GetAllAppUsersQueryHandler : IRequestHandler<GetAllAppUsersQuery, BaseResult<IReadOnlyList<RsAppUser>>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public GetAllAppUsersQueryHandler(UserManager<AppUser> userManager, UserManager<AppUserClaim> userClaimManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<BaseResult<IReadOnlyList<RsAppUser>>> Handle(GetAllAppUsersQuery request, CancellationToken cancellationToken)
            {
                var users = _mapper.Map<IReadOnlyList<RsAppUser>>(_userManager.Users);
                return BaseResult<IReadOnlyList<RsAppUser>>.Success(users);
            }
        }
    }
}
