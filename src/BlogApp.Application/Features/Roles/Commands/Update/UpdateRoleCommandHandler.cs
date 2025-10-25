using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.RoleEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Roles.Commands.Update;

public sealed class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IResult>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleCommandHandler(
        IRoleRepository roleRepository,
        IMediator mediator,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _mediator = mediator;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        // Önce mevcut rolü veritabanından getir
        var role = _roleRepository.GetRoleById(request.Id);
        if (role == null)
            return new ErrorResult("Rol bulunamadı!");

        // Aynı isimde başka bir rol var mı kontrol et (mevcut rol hariç)
        var existingRole = await _roleRepository.FindByNameAsync(request.Name);
        if (existingRole != null && existingRole.Id != request.Id)
            return new ErrorResult($"Güncellemek istediğiniz {request.Name} rolü sistemde mevcut!");

        // Mevcut role'ün property'lerini güncelle
        role.Name = request.Name;
        role.NormalizedName = request.Name.ToUpperInvariant();
        role.ConcurrencyStamp = Guid.NewGuid().ToString(); // Yeni concurrency stamp

        var result = await _roleRepository.UpdateRole(role);

        if (!result.Success)
            return new ErrorResult("İşlem sırasında bir hata oluştu");

        // ✅ Role artık AddDomainEvent() metoduna sahip (BaseEntity üzerinden)
        var currentUserId = _currentUserService.GetCurrentUserId();
        role.AddDomainEvent(new RoleUpdatedEvent(role.Id, role.Name!, currentUserId));

        // UnitOfWork SaveChanges sırasında domain event'leri otomatik olarak Outbox'a kaydeder
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Rol güncellendi.");
    }
}
