using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Permissions.Queries.GetRolePermissions;

public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, IDataResult<GetRolePermissionsResponse>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;

    public GetRolePermissionsQueryHandler(IPermissionRepository permissionRepository, IRoleRepository roleRepository)
    {
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
    }

    public async Task<IDataResult<GetRolePermissionsResponse>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        var role = _roleRepository.GetRoleById(request.RoleId);
        if (role == null)
        {
            return new ErrorDataResult<GetRolePermissionsResponse>("Rol bulunamadı");
        }

        var rolePermissions = await _permissionRepository.GetRolePermissionsAsync(request.RoleId, cancellationToken);
        var permissionIds = rolePermissions.Select(rp => rp.PermissionId).ToList();

        var response = new GetRolePermissionsResponse
        {
            RoleId = role.Id,
            RoleName = role.Name ?? string.Empty,
            PermissionIds = permissionIds
        };

        return new SuccessDataResult<GetRolePermissionsResponse>(response, "Rol permission'ları başarıyla getirildi");
    }
}
