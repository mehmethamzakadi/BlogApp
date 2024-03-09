using AutoMapper;
using BlogApp.Application.Utilities.Requests;
using BlogApp.Application.Utilities.Responses;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Queries.GetList
{
    public class GetListAppUsersQuery : IRequest<GetListResponse<GetListAppUserResponse>>
    {
        public PageRequest PageRequest { get; set; }


        public class GetAllUserQueryHandler : IRequestHandler<GetListAppUsersQuery, GetListResponse<GetListAppUserResponse>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public GetAllUserQueryHandler(UserManager<AppUser> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<GetListResponse<GetListAppUserResponse>> Handle(GetListAppUsersQuery request, CancellationToken cancellationToken)
            {

                Paginate<AppUser> userList = await _userManager.Users.ToPaginateAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
                );

                GetListResponse<GetListAppUserResponse> response = _mapper.Map<GetListResponse<GetListAppUserResponse>>(userList);

                return response;
            }
        }
    }
}
