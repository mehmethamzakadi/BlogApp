using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
                var role = await _roleRepository.GetAsync(
                    predicate: r => r.Id == roleId,
                    include: r => r.Include(x => x.UserRoles),
                    cancellationToken: cancellationToken);

                if (role == null)
                {
                    response.Errors.Add($"Rol bulunamadı: ID {roleId}");
                    response.FailedCount++;
                    continue;
                }

                if (role.NormalizedName == "ADMIN")
                {
                    response.Errors.Add($"Admin rolü silinemez");
                    response.FailedCount++;
                    continue;
                }

                if (role.UserRoles.Any(ur => !ur.IsDeleted))
                {
                    response.Errors.Add($"'{role.Name}' rolüne atanmış aktif kullanıcılar bulunmaktadır");
                    response.FailedCount++;
                    continue;
                }

                role.Delete();
                await _roleRepository.DeleteAsync(role);
                response.DeletedCount++;
            }
            catch (Exception ex)
            {
                response.Errors.Add($"Rol silinirken hata oluştu (ID {roleId}): {ex.Message}");
                response.FailedCount++;
            }
        }

        if (response.DeletedCount > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return response;
    }
}