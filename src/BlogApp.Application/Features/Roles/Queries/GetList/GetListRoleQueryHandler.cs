using AutoMapper;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Repositories;
using MediatR;


namespace BlogApp.Application.Features.Roles.Queries.GetList;

public sealed class GetListRoleQueryHandler(IRoleRepository roleRepository, IMapper mapper) : IRequestHandler<GetListRoleQuery, PaginatedListResponse<GetListRoleResponse>>
{

    public async Task<PaginatedListResponse<GetListRoleResponse>> Handle(GetListRoleQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleRepository.GetRoles(
            index: request.PageRequest.PageIndex,
            size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

        PaginatedListResponse<GetListRoleResponse> response = mapper.Map<PaginatedListResponse<GetListRoleResponse>>(roles);
        return response;
    }
}
