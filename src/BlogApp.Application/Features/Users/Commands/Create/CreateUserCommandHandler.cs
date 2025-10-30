
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Domain.Services;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Users.Commands.Create;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserDomainService _userDomainService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserDomainService userDomainService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userDomainService = userDomainService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return new ErrorResult("Bu e-posta adresi zaten kullanılıyor!");

        var existingUserName = await _userRepository.FindByUserNameAsync(request.UserName);
        if (existingUserName is not null)
            return new ErrorResult("Bu kullanıcı adı zaten kullanılıyor!");

        var user = User.Create(request.UserName, request.Email, string.Empty);
        
        var passwordResult = _userDomainService.SetPassword(user, request.Password);
        if (!passwordResult.Success)
            return passwordResult;

        await _userRepository.AddAsync(user);

        var userRole = await _roleRepository.GetAsync(r => r.NormalizedName == UserRoles.User.ToUpperInvariant());
        if (userRole != null)
        {
            var roleResult = _userDomainService.AddToRole(user, userRole);
            if (!roleResult.Success)
                return roleResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Kullanıcı bilgisi başarıyla eklendi.");
    }
}
