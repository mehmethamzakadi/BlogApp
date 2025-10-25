using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
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
    private readonly IRoleService _roleService;
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AssignPermissionsToRoleCommandHandler(
        IPermissionRepository permissionRepository,
        IRoleService roleService,
        IMediator mediator,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _permissionRepository = permissionRepository;
        _roleService = roleService;
        _mediator = mediator;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(AssignPermissionsToRoleCommand request, CancellationToken cancellationToken)
    {
        // Rol kontrolü
        var role = _roleService.GetRoleById(request.RoleId);
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

        // ✅ Domain event'i tetikle - Event handler aktiviteyi loglar
        var currentUserId = _currentUserService.GetCurrentUserId();
        await _mediator.Publish(new PermissionsAssignedToRoleEvent(role.Id, role.Name!, permissions, currentUserId), cancellationToken);

        // ✅ DÜZELTİLDİ: UnitOfWork üzerinden değişiklikleri kaydet (daha önce eksikti)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Permission'lar başarıyla atandı");
    }
}
