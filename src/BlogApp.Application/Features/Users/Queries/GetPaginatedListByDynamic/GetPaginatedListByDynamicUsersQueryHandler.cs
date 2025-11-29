using AutoMapper;
using BlogApp.Domain.Common.Dynamic;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Application.Features.Users.Queries.GetPaginatedListByDynamic;

public sealed class GetPaginatedListByDynamicUsersQueryHandler(
    IUserRepository userRepository,
    IMapper mapper) : IRequestHandler<GetPaginatedListByDynamicUsersQuery, PaginatedListResponse<GetPaginatedListByDynamicUsersResponse>>
{
    public async Task<PaginatedListResponse<GetPaginatedListByDynamicUsersResponse>> Handle(GetPaginatedListByDynamicUsersQuery request, CancellationToken cancellationToken)
    {
        Paginate<User> usersDynamic = await userRepository.GetPaginatedListByDynamicAsync(
            dynamic: request.DataGridRequest.DynamicQuery,
            index: request.DataGridRequest.PaginatedRequest.PageIndex,
            size: request.DataGridRequest.PaginatedRequest.PageSize,
            include: q => q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role),
            cancellationToken: cancellationToken
        );

        PaginatedListResponse<GetPaginatedListByDynamicUsersResponse> response = mapper.Map<PaginatedListResponse<GetPaginatedListByDynamicUsersResponse>>(usersDynamic);

        return response;
    }
}
