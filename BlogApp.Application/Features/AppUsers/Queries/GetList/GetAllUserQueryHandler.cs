using AutoMapper;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Queries.GetList;

public sealed class GetAllUserQueryHandler(UserManager<AppUser> userManager, IMapper mapper) : IRequestHandler<GetListAppUsersQuery, GetListResponse<GetListAppUserResponse>>
{
    public async Task<GetListResponse<GetListAppUserResponse>> Handle(GetListAppUsersQuery request, CancellationToken cancellationToken)
    {
        Paginate<AppUser> userList = await userManager.Users.ToPaginateAsync(
        index: request.PageRequest.PageIndex,
        size: request.PageRequest.PageSize,
        cancellationToken: cancellationToken
        );

        GetListResponse<GetListAppUserResponse> response = mapper.Map<GetListResponse<GetListAppUserResponse>>(userList);
        return response;
    }
}
