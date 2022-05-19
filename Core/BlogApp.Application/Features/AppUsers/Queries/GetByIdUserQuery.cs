using AutoMapper;
using BlogApp.Application.DTOs.AppUsers;
using BlogApp.Application.DTOs.Common;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Queries
{
    public class GetByIdUserQuery : IRequest<AppUserResponseDto>
    {
        public int Id { get; set; }

        public class GetByIdUserQueryHandler : IRequestHandler<GetByIdUserQuery, AppUserResponseDto>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public GetByIdUserQueryHandler(UserManager<AppUser> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<AppUserResponseDto> Handle(GetByIdUserQuery request, CancellationToken cancellationToken)
            {
                var response = _mapper.Map<AppUserResponseDto>(_userManager.Users.Where(x => x.Id == request.Id).FirstOrDefault());
                return response;
            }
        }
    }
}
