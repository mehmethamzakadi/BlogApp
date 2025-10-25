using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.RoleEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Roles.Commands.Create;

public sealed class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, IResult>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleCommandHandler(
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

    public async Task<IResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var checkRole = _roleRepository.AnyRole(request.Name);
        if (checkRole)
            return new ErrorResult("Eklemek istediğiniz Rol sistemde mevcut!");

        var role = new Role
        {
            Name = request.Name,
            NormalizedName = request.Name.ToUpperInvariant()
        };
        var result = await _roleRepository.CreateRole(role);

        if (!result.Success)
            return new ErrorResult("İşlem sırasında hata oluştu!");

        // ✅ Role artık AddDomainEvent() metoduna sahip (BaseEntity üzerinden)
        var currentUserId = _currentUserService.GetCurrentUserId();
        role.AddDomainEvent(new RoleCreatedEvent(role.Id, role.Name!, currentUserId));

        // UnitOfWork SaveChanges sırasında domain event'leri otomatik olarak Outbox'a kaydeder
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Rol oluşturuldu.");
    }
}
