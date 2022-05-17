using AutoMapper;
using BlogApp.Application.DTOs;
using BlogApp.Application.DTOs.Common;
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
    public class GetAllUsersQuery : IRequest<BaseResult<IReadOnlyList<AppUserDto>>>
    {

        public class GetAllUserQueryHandler : IRequestHandler<GetAllUsersQuery, BaseResult<IReadOnlyList<AppUserDto>>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public GetAllUserQueryHandler(UserManager<AppUser> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<BaseResult<IReadOnlyList<AppUserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
            {
                return BaseResult<IReadOnlyList<AppUserDto>>.Success(_mapper.Map<IReadOnlyList<AppUserDto>>(_userManager.Users.ToList()));
            }
        }
    }
}
