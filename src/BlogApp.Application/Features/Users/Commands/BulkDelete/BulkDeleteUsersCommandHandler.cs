using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Users.Commands.BulkDelete;

public class BulkDeleteUsersCommandHandler : IRequestHandler<BulkDeleteUsersCommand, BulkDeleteUsersResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public BulkDeleteUsersCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<BulkDeleteUsersResponse> Handle(BulkDeleteUsersCommand request, CancellationToken cancellationToken)
    {
        var response = new BulkDeleteUsersResponse();

        foreach (var userId in request.UserIds)
        {
            try
            {
                var user = await _userRepository.FindByIdAsync(userId);

                if (user == null)
                {
                    response.Errors.Add($"Kullanıcı bulunamadı: ID {userId}");
                    response.FailedCount++;
                    continue;
                }

                user.Delete();
                _userRepository.Delete(user);
                response.DeletedCount++;
            }
            catch (Exception ex)
            {
                response.Errors.Add($"Kullanıcı silinirken hata oluştu (ID {userId}): {ex.Message}");
                response.FailedCount++;
            }
        }

        // Tüm değişiklikleri tek transaction'da kaydet (Silme işlemleri + Outbox)
        if (response.DeletedCount > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return response;
    }
}
