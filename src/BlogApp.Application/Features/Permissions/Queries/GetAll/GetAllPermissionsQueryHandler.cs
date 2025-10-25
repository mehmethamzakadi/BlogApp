using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Permissions.Queries.GetAll;

public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, IDataResult<GetAllPermissionsResponse>>
{
    private readonly IPermissionRepository _permissionRepository;

    public GetAllPermissionsQueryHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<IDataResult<GetAllPermissionsResponse>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
    {
        // Tüm permission'ları al
        var permissions = await _permissionRepository.GetAllAsync(
            predicate: p => !p.IsDeleted,
            orderBy: q => q.OrderBy(p => p.Module).ThenBy(p => p.Type),
            enableTracking: false,
            cancellationToken: cancellationToken
        );

        var grouped = permissions
            .GroupBy(p => p.Module)
            .Select(g => new PermissionModuleDto
            {
                ModuleName = g.Key,
                Permissions = g.Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description ?? string.Empty,
                    Type = p.Type
                }).ToList()
            })
            .OrderBy(m => m.ModuleName)
            .ToList();

        var response = new GetAllPermissionsResponse
        {
            Modules = grouped
        };

        return new SuccessDataResult<GetAllPermissionsResponse>(response, "Permission'lar başarıyla getirildi");
    }
}
