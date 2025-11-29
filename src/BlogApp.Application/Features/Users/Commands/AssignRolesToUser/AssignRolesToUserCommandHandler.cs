using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Events.UserEvents;
using BlogApp.Domain.Repositories;
using MediatR;
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

        var requestedRoleIds = request.RoleIds.ToHashSet();

        // Mevcut UserRole'leri al (silinmiş olanlar dahil)
        var existingUserRoles = await _userRepository.GetAllUserRolesAsync(request.UserId, cancellationToken);

        var existingRoleIds = existingUserRoles
            .Where(ur => !ur.IsDeleted)
            .Select(ur => ur.RoleId)
            .ToHashSet();

        // Silinecek roller (mevcut ama istenen listede yok)
        var rolesToRemove = existingRoleIds.Except(requestedRoleIds).ToList();

        // Eklenecek roller (istenen listede var ama mevcut değil)
        var rolesToAdd = requestedRoleIds.Except(existingRoleIds).ToList();

        // Değişiklik yoksa erken çık
        if (!rolesToRemove.Any() && !rolesToAdd.Any())
        {
            return new SuccessResult("Roller zaten güncel");
        }

        // Silinecek rolleri soft delete yap
        if (rolesToRemove.Any())
        {
            var userRolesToRemove = existingUserRoles
                .Where(ur => rolesToRemove.Contains(ur.RoleId) && !ur.IsDeleted)
                .ToList();

            await _userRepository.SoftDeleteUserRolesAsync(userRolesToRemove, cancellationToken);
        }

        // Eklenecek rolleri ekle veya geri ekle
        if (rolesToAdd.Any())
        {
            foreach (var roleId in rolesToAdd)
            {
                // Önce silinmiş bir UserRole var mı kontrol et
                var deletedUserRole = existingUserRoles
                    .FirstOrDefault(ur => ur.RoleId == roleId && ur.IsDeleted);

                if (deletedUserRole != null)
                {
                    // Silinmiş rolü geri ekle
                    await _userRepository.RestoreUserRoleAsync(deletedUserRole, cancellationToken);
                }
                else
                {
                    // Yeni UserRole oluştur
                    await _userRepository.AddUserRoleAsync(request.UserId, roleId, cancellationToken);
                }
            }
        }

        // Domain Event
        var allCurrentRoleNames = await _userRepository.GetRolesAsync(user);
        user.AddDomainEvent(new UserRolesAssignedEvent(user.Id, user.UserName!, allCurrentRoleNames));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Roller başarıyla atandı");
    }
}