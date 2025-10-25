using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.UserEvents;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.AppUsers.Commands.Update;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateAppUserCommand, IResult>
{
    private readonly IUserService _userManager;
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUserCommandHandler(
        IUserService userManager,
        IMediator mediator,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public async Task<IResult> Handle(UpdateAppUserCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = _userManager.FindById(request.Id);
        if (user is null)
            return new ErrorResult("Kullanıcı Bilgisi Bulunamadı!");

        user.Email = request.Email;
        user.UserName = request.UserName;

        var response = await _userManager.UpdateAsync(user);
        if (!response.Succeeded)
            return new ErrorResult("Güncelleme işlemi sırasında hata oluştu!");

        // ✅ Raise domain event - Event handler will log the activity
        var currentUserId = _currentUserService.GetCurrentUserId();
        await _mediator.Publish(new UserUpdatedEvent(user.Id, user.UserName!, user.Email!, currentUserId), cancellationToken);

        return new SuccessResult("Kullanıcı bilgisi başarıyla güncellendi.");
    }
}
