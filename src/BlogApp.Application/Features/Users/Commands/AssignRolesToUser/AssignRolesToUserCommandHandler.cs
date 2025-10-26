using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.UserEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Users.Commands.AssignRolesToUser;

public class AssignRolesToUserCommandHandler : IRequestHandler<AssignRolesToUserCommand, IResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRolesToUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(AssignRolesToUserCommand request, CancellationToken cancellationToken)
    {
        // Kullanıcı kontrolü
        var user = await _userRepository.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return new ErrorResult("Kullanıcı bulunamadı");
        }

        // Var olan rolleri al ve sil
        var currentRoles = await _userRepository.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            var removeResult = await _userRepository.RemoveFromRolesAsync(user, currentRoles.ToArray());
            if (!removeResult.Success)
            {
                return new ErrorResult("Mevcut roller kaldırılamadı");
            }
        }

        // Yeni rolleri ekle
        if (request.RoleIds.Any())
        {
            var roles = await _roleRepository.Query()
                .Where(r => request.RoleIds.Contains(r.Id))
                .Select(r => r.Name!)
                .ToListAsync(cancellationToken);

            if (!roles.Any())
            {
                return new ErrorResult("Geçerli rol bulunamadı");
            }

            var addResult = await _userRepository.AddToRolesAsync(user, roles.ToArray());
            if (!addResult.Success)
            {
                return new ErrorResult("Roller eklenemedi: " + addResult.Message);
            }

            // ✅ User artık AddDomainEvent() metoduna sahip (BaseEntity üzerinden)
            var currentUserId = _currentUserService.GetCurrentUserId();
            user.AddDomainEvent(new UserRolesAssignedEvent(user.Id, user.UserName!, roles, currentUserId));
        }

        // UnitOfWork SaveChanges sırasında domain event'leri otomatik olarak Outbox'a kaydeder
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Roller başarıyla atandı");
    }
}
