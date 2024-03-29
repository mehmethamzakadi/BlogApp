using AutoMapper;
using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Queries.GetList;

public sealed class GetListUserQueryHandler(IUserService userManager, IMapper mapper) : IRequestHandler<GetListAppUsersQuery, GetListResponse<GetListAppUserResponse>>
{
    public async Task<GetListResponse<GetListAppUserResponse>> Handle(GetListAppUsersQuery request, CancellationToken cancellationToken)
    {
        Paginate<AppUser> userList = await userManager.GetUsers(
        index: request.PageRequest.PageIndex,
        size: request.PageRequest.PageSize,
        cancellationToken: cancellationToken
        );

        GetListResponse<GetListAppUserResponse> response = mapper.Map<GetListResponse<GetListAppUserResponse>>(userList);
        return response;
    }
}
