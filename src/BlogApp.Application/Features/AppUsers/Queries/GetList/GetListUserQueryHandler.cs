using AutoMapper;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Queries.GetList;

public sealed class GetListUserQueryHandler(IUserService userManager, IMapper mapper) : IRequestHandler<GetListAppUsersQuery, PaginatedListResponse<GetListAppUserResponse>>
{
    public async Task<PaginatedListResponse<GetListAppUserResponse>> Handle(GetListAppUsersQuery request, CancellationToken cancellationToken)
    {
        Paginate<AppUser> userList = await userManager.GetUsers(
        index: request.PageRequest.PageIndex,
        size: request.PageRequest.PageSize,
        cancellationToken: cancellationToken
        );

        PaginatedListResponse<GetListAppUserResponse> response = mapper.Map<PaginatedListResponse<GetListAppUserResponse>>(userList);
        return response;
    }
}
