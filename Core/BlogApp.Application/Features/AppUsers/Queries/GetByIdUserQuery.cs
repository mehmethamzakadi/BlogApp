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
    public class GetByIdUserQuery : IRequest<BaseResult<AppUserDto>>
    {
        public int Id { get; set; }

        public class GetByIdUserQueryHandler : IRequestHandler<GetByIdUserQuery, BaseResult<AppUserDto>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public GetByIdUserQueryHandler(UserManager<AppUser> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<BaseResult<AppUserDto>> Handle(GetByIdUserQuery request, CancellationToken cancellationToken)
            {
                return BaseResult<AppUserDto>.Success(_mapper.Map<AppUserDto>(_userManager.Users.Where(x => x.Id == request.Id).FirstOrDefault()));
            }
        }
    }
}
