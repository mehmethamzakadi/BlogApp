using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.RoleEvents;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.AppRoles.Commands.Update;

public sealed class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IResult>
{
    private readonly IRoleService _roleService;
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateRoleCommandHandler(
        IRoleService roleService,
        IMediator mediator,
        IHttpContextAccessor httpContextAccessor)
    {
        _roleService = roleService;
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
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

        // ✅ Raise domain event - Event handler will log the activity
        var currentUserId = GetCurrentUserId();
        await _mediator.Publish(new RoleUpdatedEvent(role.Id, role.Name!, currentUserId), cancellationToken);

        return new SuccessResult("Rol güncellendi.");
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