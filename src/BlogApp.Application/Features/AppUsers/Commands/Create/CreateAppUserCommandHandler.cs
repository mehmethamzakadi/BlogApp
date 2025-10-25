
using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.UserEvents;
using MediatR;
using Microsoft.AspNetCore.Identity;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.AppUsers.Commands.Create;

public sealed class CreateAppUserCommandHandler : IRequestHandler<CreateAppUserCommand, IResult>
{
    private readonly IUserService _userService;
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public CreateAppUserCommandHandler(
        IUserService userService,
        IMediator mediator,
        ICurrentUserService currentUserService)
    {
        _userService = userService;
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public async Task<IResult> Handle(CreateAppUserCommand request, CancellationToken cancellationToken)
    {
        AppUser? existingUser = await _userService.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return new ErrorResult("Böyle bir kullanıcı zaten sistemde mevcut!");
        }

        var user = new AppUser
        {
            Email = request.Email,
            UserName = request.UserName,
        };

        IdentityResult creationResult = await _userService.CreateAsync(user, request.Password);
        if (!creationResult.Succeeded)
        {
            List<string> errors = creationResult.Errors.Select(error => error.Description).ToList();
            string message = "Kullanıcı oluşturulurken hatalar oluştu";

            return new ErrorResult(message, errors);
        }

        await _userService.AddToRoleAsync(user, UserRoles.User);

        // ✅ Raise domain event - Event handler will log the activity
        var currentUserId = _currentUserService.GetCurrentUserId();
        await _mediator.Publish(new UserCreatedEvent(user.Id, user.UserName!, user.Email!, currentUserId), cancellationToken);

        return new SuccessResult("Kullanıcı bilgisi başarıyla eklendi.");
    }
}
