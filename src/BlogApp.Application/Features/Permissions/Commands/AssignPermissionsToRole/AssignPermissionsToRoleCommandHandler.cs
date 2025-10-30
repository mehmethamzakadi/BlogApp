using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Events.PermissionEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Permissions.Commands.AssignPermissionsToRole;

public class AssignPermissionsToRoleCommandHandler : IRequestHandler<AssignPermissionsToRoleCommand, IResult>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AssignPermissionsToRoleCommandHandler(
        IPermissionRepository permissionRepository,
        IRoleRepository roleRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(AssignPermissionsToRoleCommand request, CancellationToken cancellationToken)
    {
        // Rol kontrolü
        var role = _roleRepository.GetRoleById(request.RoleId);
        if (role == null)
        {
            return new ErrorResult("Rol bulunamadı");
        }

        // Permission bilgilerini al (event için)
        var permissions = await _permissionRepository.Query()
            .Where(p => request.PermissionIds.Contains(p.Id))
            .Select(p => p.Name)
            .ToListAsync(cancellationToken);

        // Repository üzerinden permission'ları ata
        await _permissionRepository.AssignPermissionsToRoleAsync(request.RoleId, request.PermissionIds, cancellationToken);

        // Domain event ekle
        role.AddDomainEvent(new PermissionsAssignedToRoleEvent(role.Id, role.Name!, permissions));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Permission'lar başarıyla atandı");
    }
}