using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Events.RoleEvents;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Roles.Commands.BulkDelete;

public class BulkDeleteRolesCommandHandler : IRequestHandler<BulkDeleteRolesCommand, BulkDeleteRolesResponse>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public BulkDeleteRolesCommandHandler(
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _roleRepository = roleRepository;
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
                var role = _roleRepository.GetRoleById(roleId);

                if (role == null)
                {
                    response.Errors.Add($"Rol bulunamadı: ID {roleId}");
                    response.FailedCount++;
                    continue;
                }

                // Admin rolü silinemez (case-insensitive kontrol)
                if (role.NormalizedName == "ADMIN")
                {
                    response.Errors.Add($"Admin rolü silinemez");
                    response.FailedCount++;
                    continue;
                }

                // ✅ Silme işleminden ÖNCE domain event'i tetikle
                var currentUserId = _currentUserService.GetCurrentUserId();
                role.AddDomainEvent(new RoleDeletedEvent(roleId, role.Name!, currentUserId));

                var result = await _roleRepository.DeleteRole(role);

                if (result.Success)
                {
                    response.DeletedCount++;
                }
                else
                {
                    response.Errors.Add($"Rol silinemedi (ID {roleId}): {result.Message}");
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
