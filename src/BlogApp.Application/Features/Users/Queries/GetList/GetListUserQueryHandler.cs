using AutoMapper;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Users.Queries.GetList;

public sealed class GetListUserQueryHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<GetListUsersQuery, PaginatedListResponse<GetListUserResponse>>
{
    public async Task<PaginatedListResponse<GetListUserResponse>> Handle(GetListUsersQuery request, CancellationToken cancellationToken)
    {
        Paginate<User> userList = await userRepository.GetUsersAsync(
        index: request.PageRequest.PageIndex,
        size: request.PageRequest.PageSize,
        cancellationToken: cancellationToken
        );

        PaginatedListResponse<GetListUserResponse> response = mapper.Map<PaginatedListResponse<GetListUserResponse>>(userList);
        return response;
    }
}
