using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Events.RoleEvents;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.AppRoles.Commands.Delete;

public sealed class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, IResult>
{
    private readonly IRoleService _roleService;
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteRoleCommandHandler(
        IRoleService roleService,
        IMediator mediator,
        IHttpContextAccessor httpContextAccessor)
    {
        _roleService = roleService;
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
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

        // ✅ Domain event'i tetikle - Event handler aktiviteyi loglar
        var currentUserId = GetCurrentUserId();
        await _mediator.Publish(new RoleDeletedEvent(roleId, roleName, currentUserId), cancellationToken);

        return new SuccessResult("Rol silindi.");
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
}