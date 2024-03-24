using BlogApp.Application.Abstractions;
using BlogApp.Domain.Entities;
using MediatR;


namespace BlogApp.Application.Features.AppRoles.Queries.GetRoleById;

public class GetRoleByIdQueryHandler(IRoleService roleService) : IRequestHandler<GetRoleByIdQueryRequest, GetRoleByIdQueryResponse>
{
    public async Task<GetRoleByIdQueryResponse> Handle(GetRoleByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var roleName = await roleService.GetRoleById(new AppRole { Id = request.Id });
        return new GetRoleByIdQueryResponse(Id: request.Id, Name: roleName);
    }
}