using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Events.UserEvents;
using BlogApp.Domain.Repositories;
using BlogApp.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Users.Commands.AssignRolesToUser;

public class AssignRolesToUserCommandHandler : IRequestHandler<AssignRolesToUserCommand, IResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserDomainService _userDomainService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRolesToUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserDomainService userDomainService,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userDomainService = userDomainService;
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

        // ✅ BEST PRACTICE: Delta Update - Sadece değişenleri güncelle
        var currentUserRoles = await _userRepository.GetUserRoleIdsAsync(user.Id, cancellationToken);
        var requestedRoleIds = request.RoleIds.ToHashSet();
        var currentRoleIds = currentUserRoles.ToHashSet();

        // Silinecek roller (mevcut ama istenen listede yok)
        var rolesToRemove = currentRoleIds.Except(requestedRoleIds).ToList();

        // Eklenecek roller (istenen listede var ama mevcut değil)
        var rolesToAdd = requestedRoleIds.Except(currentRoleIds).ToList();

        // Değişiklik yoksa erken çık
        if (!rolesToRemove.Any() && !rolesToAdd.Any())
        {
            return new SuccessResult("Roller zaten güncel");
        }

        // Silinecek rolleri al
        if (rolesToRemove.Any())
        {
            var rolesToRemoveEntities = await _roleRepository.Query()
                .Where(r => rolesToRemove.Contains(r.Id))
                .ToListAsync(cancellationToken);

            var removeResult = _userDomainService.RemoveFromRoles(user, rolesToRemoveEntities);
            if (!removeResult.Success)
            {
                return new ErrorResult("Roller kaldırılamadı: " + removeResult.Message);
            }
        }

        // Eklenecek rolleri al
        if (rolesToAdd.Any())
        {
            var rolesToAddEntities = await _roleRepository.Query()
                .Where(r => rolesToAdd.Contains(r.Id))
                .ToListAsync(cancellationToken);

            if (rolesToAddEntities.Count != rolesToAdd.Count)
            {
                return new ErrorResult("Bazı roller bulunamadı");
            }

            var addResult = _userDomainService.AddToRoles(user, rolesToAddEntities);
            if (!addResult.Success)
            {
                return new ErrorResult("Roller eklenemedi: " + addResult.Message);
            }

            // ✅ Domain Event - sadece değişiklik olduğunda
            var currentUserId = _currentUserService.GetCurrentUserId();

            // Tüm rollerin son halini al (event için)
            var allCurrentRoleNames = await _userRepository.GetRolesAsync(user);
            user.AddDomainEvent(new UserRolesAssignedEvent(user.Id, user.UserName!, allCurrentRoleNames, currentUserId));
        }

        // UnitOfWork SaveChanges sırasında domain event'leri otomatik olarak Outbox'a kaydeder
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Roller başarıyla atandı");
    }
}
