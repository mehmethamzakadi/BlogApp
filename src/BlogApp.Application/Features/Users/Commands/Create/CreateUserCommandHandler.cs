
using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.UserEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Users.Commands.Create;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IMediator mediator,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _mediator = mediator;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Email kontrolü (case-insensitive)
        User? existingUser = await _userRepository.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return new ErrorResult("Bu e-posta adresi zaten kullanılıyor!");
        }

        // Username kontrolü (case-insensitive)
        User? existingUserName = await _userRepository.FindByUserNameAsync(request.UserName);
        if (existingUserName is not null)
        {
            return new ErrorResult("Bu kullanıcı adı zaten kullanılıyor!");
        }

        var user = new User
        {
            Email = request.Email,
            UserName = request.UserName,
            NormalizedUserName = request.UserName.ToUpperInvariant(),
            NormalizedEmail = request.Email.ToUpperInvariant(),
            PasswordHash = string.Empty // CreateAsync içinde set edilecek
        };

        var creationResult = await _userRepository.CreateAsync(user, request.Password);
        if (!creationResult.Success)
        {
            return creationResult;
        }

        // ✅ Domain event'i ekleme işleminden ÖNCE tetikle
        var currentUserId = _currentUserService.GetCurrentUserId();
        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.UserName!, user.Email!, currentUserId));

        await _userRepository.AddToRoleAsync(user, UserRoles.User);

        // UnitOfWork SaveChanges sırasında domain event'leri otomatik olarak Outbox'a kaydeder
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Kullanıcı bilgisi başarıyla eklendi.");
    }
}
