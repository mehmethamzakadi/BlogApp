
using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.UserEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.AppUsers.Commands.Create;

public sealed class CreateAppUserCommandHandler : IRequestHandler<CreateAppUserCommand, IResult>
{
    private readonly IUserService _userService;
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAppUserCommandHandler(
        IUserService userService,
        IMediator mediator,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _userService = userService;
        _mediator = mediator;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
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

        // ✅ AppUser artık AddDomainEvent() metoduna sahip (IHasDomainEvents sayesinde)
        var currentUserId = _currentUserService.GetCurrentUserId();
        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.UserName!, user.Email!, currentUserId));

        // UnitOfWork SaveChanges sırasında domain event'leri otomatik olarak Outbox'a kaydeder
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Kullanıcı bilgisi başarıyla eklendi.");
    }
}
