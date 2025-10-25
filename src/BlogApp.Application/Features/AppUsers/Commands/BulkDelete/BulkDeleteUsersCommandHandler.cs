using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.UserEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Commands.BulkDelete;

public class BulkDeleteUsersCommandHandler : IRequestHandler<BulkDeleteUsersCommand, BulkDeleteUsersResponse>
{
    private readonly IUserService _userService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public BulkDeleteUsersCommandHandler(
        IUserService userService,
        UserManager<AppUser> userManager,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _userService = userService;
        _userManager = userManager;
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
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    response.Errors.Add($"Kullanıcı bulunamadı: ID {userId}");
                    response.FailedCount++;
                    continue;
                }

                // Event için bilgileri sakla (silindikten sonra erişemeyebiliriz)
                var userName = user.UserName ?? "";
                var userEmail = user.Email ?? "";

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    // ✅ AppUser artık AddDomainEvent() metoduna sahip
                    var currentUserId = _currentUserService.GetCurrentUserId();
                    user.AddDomainEvent(new UserDeletedEvent(userId, userName, userEmail, currentUserId));
                    
                    response.DeletedCount++;
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    response.Errors.Add($"Kullanıcı silinemedi (ID {userId}): {errors}");
                    response.FailedCount++;
                }
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
