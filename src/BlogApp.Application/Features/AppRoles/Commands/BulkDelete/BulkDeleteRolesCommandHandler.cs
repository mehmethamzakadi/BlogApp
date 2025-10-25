using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.RoleEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppRoles.Commands.BulkDelete;

public class BulkDeleteRolesCommandHandler : IRequestHandler<BulkDeleteRolesCommand, BulkDeleteRolesResponse>
{
    private readonly IRoleService _roleService;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public BulkDeleteRolesCommandHandler(
        IRoleService roleService,
        RoleManager<AppRole> roleManager,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _roleService = roleService;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<BulkDeleteRolesResponse> Handle(BulkDeleteRolesCommand request, CancellationToken cancellationToken)
    {
        var response = new BulkDeleteRolesResponse();

        foreach (var roleId in request.RoleIds)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());

                if (role == null)
                {
                    response.Errors.Add($"Rol bulunamadı: ID {roleId}");
                    response.FailedCount++;
                    continue;
                }

                // Admin rolü silinemez
                if (role.Name == "Admin")
                {
                    response.Errors.Add($"Admin rolü silinemez");
                    response.FailedCount++;
                    continue;
                }

                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    // ✅ AppRole artık AddDomainEvent() metoduna sahip
                    var currentUserId = _currentUserService.GetCurrentUserId();
                    role.AddDomainEvent(new RoleDeletedEvent(roleId, role.Name!, currentUserId));
                    
                    response.DeletedCount++;
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    response.Errors.Add($"Rol silinemedi (ID {roleId}): {errors}");
                    response.FailedCount++;
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add($"Rol silinirken hata oluştu (ID {roleId}): {ex.Message}");
                response.FailedCount++;
            }
        }

        // Tüm değişiklikleri tek transaction'da kaydet (Silme işlemleri + Outbox)
        if (response.DeletedCount > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return response;
    }
}
