using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.RoleEvents;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.AppRoles.Commands.Create;

public sealed class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, IResult>
{
    private readonly IRoleService _roleService;
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateRoleCommandHandler(
        IRoleService roleService,
        IMediator mediator,
        IHttpContextAccessor httpContextAccessor)
    {
        _roleService = roleService;
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var checkRole = _roleService.AnyRole(request.Name);
        if (checkRole)
            return new ErrorResult("Eklemek istediğiniz Rol sistemde mevcut!");

        var role = new AppRole { Name = request.Name };
        var result = await _roleService.CreateRole(role);

        if (!result.Succeeded)
            return new ErrorResult("İşlem sırasında hata oluştu!");

        // ✅ Domain event'i tetikle - Event handler aktiviteyi loglar
        var currentUserId = GetCurrentUserId();
        await _mediator.Publish(new RoleCreatedEvent(role.Id, role.Name!, currentUserId), cancellationToken);

        return new SuccessResult("Rol oluşturuldu.");
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