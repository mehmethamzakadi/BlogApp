using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Users.Commands.Update;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByIdAsync(request.Id);
        if (user is null)
            return new ErrorResult("Kullanıcı Bilgisi Bulunamadı!");

        if (user.Email != request.Email)
        {
            var existingEmail = await _userRepository.FindByEmailAsync(request.Email);
            if (existingEmail != null && existingEmail.Id != request.Id)
                return new ErrorResult("Bu e-posta adresi zaten kullanılıyor!");
        }

        if (user.UserName != request.UserName)
        {
            var existingUserName = await _userRepository.FindByUserNameAsync(request.UserName);
            if (existingUserName != null && existingUserName.Id != request.Id)
                return new ErrorResult("Bu kullanıcı adı zaten kullanılıyor!");
        }

        user.Update(request.UserName, request.Email);
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Kullanıcı bilgisi başarıyla güncellendi.");
    }
}
