using BlogApp.Application.Abstractions;
using MediatR;


namespace BlogApp.Application.Features.AppRoles.Queries.GetAllRoles;

public class GetAllRoleQueryHandler(IRoleService roleService) : IRequestHandler<GetAllRoleQueryRequest, GetAllRoleQueryResponse>
{

    public async Task<GetAllRoleQueryResponse> Handle(GetAllRoleQueryRequest request, CancellationToken cancellationToken)
    {
        var roles = roleService.GetAllRoles();
        return new GetAllRoleQueryResponse(roles);
    }
}