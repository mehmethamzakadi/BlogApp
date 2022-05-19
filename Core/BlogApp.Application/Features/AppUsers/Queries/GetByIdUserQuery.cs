using AutoMapper;
using BlogApp.Application.DTOs.AppUsers;
using BlogApp.Application.Utilities.Results;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Queries
{
    public class GetByIdUserQuery : IRequest<IDataResult<AppUserResponseDto>>
    {
        public int Id { get; set; }

        public class GetByIdUserQueryHandler : IRequestHandler<GetByIdUserQuery, IDataResult<AppUserResponseDto>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public GetByIdUserQueryHandler(UserManager<AppUser> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<IDataResult<AppUserResponseDto>> Handle(GetByIdUserQuery request, CancellationToken cancellationToken)
            {
                var user = _userManager.Users.Where(x => x.Id == request.Id).FirstOrDefault();
                var userDto = _mapper.Map<AppUserResponseDto>(user);
                return new SuccessDataResult<AppUserResponseDto>(userDto);
            }
        }
    }
}
