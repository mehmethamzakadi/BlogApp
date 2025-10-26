using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Events.RoleEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Roles.Commands.Delete;

public sealed class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, IResult>
{
    private readonly IRoleRepository _roleRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoleCommandHandler(
        IRoleRepository roleRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = _roleRepository.GetRoleById(request.Id);
        if (role == null)
            return new ErrorResult("Rol bulunamadı!");

        // Admin rolü silinemez (case-insensitive kontrol)
        if (role.NormalizedName == "ADMIN")
            return new ErrorResult("Admin rolü silinemez!");

        // ✅ Silme işleminden ÖNCE domain event'i tetikle
        var roleId = role.Id;
        var roleName = role.Name ?? "";
        var currentUserId = _currentUserService.GetCurrentUserId();
        role.AddDomainEvent(new RoleDeletedEvent(roleId, roleName, currentUserId));

        var result = await _roleRepository.DeleteRole(role);
        if (!result.Success)
            return new ErrorResult("Rol silme sırasında hata oluştu!");

        // UnitOfWork SaveChanges sırasında domain event'leri otomatik olarak Outbox'a kaydeder
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Rol silindi.");
    }
}
