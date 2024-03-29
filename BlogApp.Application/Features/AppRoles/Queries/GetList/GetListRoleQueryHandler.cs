using AutoMapper;
using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Responses;
using MediatR;


namespace BlogApp.Application.Features.AppRoles.Queries.GetList;

public class GetListRoleQueryHandler(IRoleService roleService, IMapper mapper) : IRequestHandler<GetListRoleQuery, GetListResponse<GetListAppRoleResponse>>
{

    public async Task<GetListResponse<GetListAppRoleResponse>> Handle(GetListRoleQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleService.GetRoles(
            index: request.PageRequest.PageIndex,
            size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);

        GetListResponse<GetListAppRoleResponse> response = mapper.Map<GetListResponse<GetListAppRoleResponse>>(roles);
        return response;
    }
}