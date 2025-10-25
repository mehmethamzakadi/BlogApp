using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.UserEvents;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.AppUsers.Commands.AssignRolesToUser;

public class AssignRolesToUserCommandHandler : IRequestHandler<AssignRolesToUserCommand, IResult>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public AssignRolesToUserCommandHandler(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IMediator mediator,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public async Task<IResult> Handle(AssignRolesToUserCommand request, CancellationToken cancellationToken)
    {
        // Kullanıcı kontrolü
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return new ErrorResult("Kullanıcı bulunamadı");
        }

        // Var olan rolleri al ve sil
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return new ErrorResult("Mevcut roller kaldırılamadı");
            }
        }

        // Yeni rolleri ekle
        if (request.RoleIds.Any())
        {
            var roles = await _roleManager.Roles
                .Where(r => request.RoleIds.Contains(r.Id))
                .Select(r => r.Name!)
                .ToListAsync(cancellationToken);

            if (!roles.Any())
            {
                return new ErrorResult("Geçerli rol bulunamadı");
            }

            var addResult = await _userManager.AddToRolesAsync(user, roles);
            if (!addResult.Succeeded)
            {
                return new ErrorResult("Roller eklenemedi: " + string.Join(", ", addResult.Errors.Select(e => e.Description)));
            }

            // ✅ Raise domain event - Event handler will log the activity
            var currentUserId = _currentUserService.GetCurrentUserId();
            await _mediator.Publish(new UserRolesAssignedEvent(user.Id, user.UserName!, roles, currentUserId), cancellationToken);
        }

        return new SuccessResult("Roller başarıyla atandı");
    }
}
