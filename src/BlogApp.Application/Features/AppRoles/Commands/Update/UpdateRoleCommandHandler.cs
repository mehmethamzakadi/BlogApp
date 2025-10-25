using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.RoleEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.AppRoles.Commands.Update;

public sealed class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IResult>
{
    private readonly IRoleService _roleService;
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleCommandHandler(
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

    public async Task<IResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var checkRole = _roleService.AnyRole(request.Name);
        if (checkRole)
            return new ErrorResult($"Güncellemek istediğiniz {request.Name} rolü sistemde mevcut!");

        var role = new AppRole { Id = request.Id, Name = request.Name };
        var result = await _roleService.UpdateRole(role);

        if (!result.Succeeded)
            return new ErrorResult("İşlem sırasında bir hata oluştu");

        // ✅ AppRole artık AddDomainEvent() metoduna sahip
        var currentUserId = _currentUserService.GetCurrentUserId();
        role.AddDomainEvent(new RoleUpdatedEvent(role.Id, role.Name!, currentUserId));

        // UnitOfWork SaveChanges sırasında domain event'leri otomatik olarak Outbox'a kaydeder
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Rol güncellendi.");
    }
}