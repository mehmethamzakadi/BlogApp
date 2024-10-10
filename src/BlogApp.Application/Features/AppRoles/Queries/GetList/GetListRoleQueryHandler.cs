using AutoMapper;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Responses;
using MediatR;


namespace BlogApp.Application.Features.AppRoles.Queries.GetList;

public class GetListRoleQueryHandler(IRoleService roleService, IMapper mapper) : IRequestHandler<GetListRoleQuery, PaginatedListResponse<GetListAppRoleResponse>>
{

    public async Task<PaginatedListResponse<GetListAppRoleResponse>> Handle(GetListRoleQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleService.GetRoles(
            index: request.PageRequest.PageIndex,
            size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

        PaginatedListResponse<GetListAppRoleResponse> response = mapper.Map<PaginatedListResponse<GetListAppRoleResponse>>(roles);
        return response;
    }
}