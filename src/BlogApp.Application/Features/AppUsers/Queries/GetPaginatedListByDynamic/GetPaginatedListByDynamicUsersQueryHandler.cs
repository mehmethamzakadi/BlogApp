using AutoMapper;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Queries.GetPaginatedListByDynamic;

public sealed class GetPaginatedListByDynamicUsersQueryHandler(
    IUserService userManager,
    IMapper mapper) : IRequestHandler<GetPaginatedListByDynamicUsersQuery, PaginatedListResponse<GetPaginatedListByDynamicUsersResponse>>
{
    public async Task<PaginatedListResponse<GetPaginatedListByDynamicUsersResponse>> Handle(GetPaginatedListByDynamicUsersQuery request, CancellationToken cancellationToken)
    {
        Paginate<AppUser> usersDynamic = await userManager.GetUsers(
        index: request.DataGridRequest.PaginatedRequest.PageIndex,
        size: request.DataGridRequest.PaginatedRequest.PageSize,
        cancellationToken: cancellationToken
        );

        PaginatedListResponse<GetPaginatedListByDynamicUsersResponse> response = mapper.Map<PaginatedListResponse<GetPaginatedListByDynamicUsersResponse>>(usersDynamic);

        return response;
    }
}
