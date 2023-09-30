using AutoMapper;
using BlogApp.Application.DTOs.AppUsers;
using BlogApp.Application.Utilities.Requests;
using BlogApp.Application.Utilities.Results;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Queries.GetList
{
    public class GetListAppUsersQuery : IRequest<IDataResult<IReadOnlyList<GetListAppUserResponse>>>
    {
        public PageRequest PageRequest { get; set; }


        public class GetAllUserQueryHandler : IRequestHandler<GetListAppUsersQuery, IDataResult<IReadOnlyList<GetListAppUserResponse>>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public GetAllUserQueryHandler(UserManager<AppUser> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<IDataResult<IReadOnlyList<GetListAppUserResponse>>> Handle(GetListAppUsersQuery request, CancellationToken cancellationToken)
            {
                var userList = _userManager.Users;
                var response = _mapper.Map<IReadOnlyList<GetListAppUserResponse>>(userList).ToList();
                return new SuccessDataResult<IReadOnlyList<GetListAppUserResponse>>(response);
            }
        }
    }
}
