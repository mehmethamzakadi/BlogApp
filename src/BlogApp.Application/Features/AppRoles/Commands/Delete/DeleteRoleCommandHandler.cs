using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.RoleEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.AppRoles.Commands.Delete;

public sealed class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, IResult>
{
    private readonly IRoleService _roleService;
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoleCommandHandler(
        IRoleService roleService,
        IMediator mediator,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _roleService = roleService;
        _mediator = mediator;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        // Event için role bilgisini al
        var role = _roleService.GetRoleById(request.Id);
        if (role == null)
            return new ErrorResult("Rol bulunamadı!");

        var roleId = role.Id;
        var roleName = role.Name ?? "";

        var result = await _roleService.DeleteRole(role);
        if (!result.Succeeded)
            return new ErrorResult("Rol silme sırasında hata oluştu!");

        // ✅ AppRole artık AddDomainEvent() metoduna sahip
        var currentUserId = _currentUserService.GetCurrentUserId();
        role.AddDomainEvent(new RoleDeletedEvent(roleId, roleName, currentUserId));

        // UnitOfWork SaveChanges sırasında domain event'leri otomatik olarak Outbox'a kaydeder
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Rol silindi.");
    }
}